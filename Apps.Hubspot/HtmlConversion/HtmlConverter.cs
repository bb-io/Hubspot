using System.Text;
using System.Web;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Entities;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Pages;
using Apps.Hubspot.Providers;
using Apps.Hubspot.Services.ContentServices;
using Apps.Hubspot.Utils.Extensions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.HtmlConversion;

public static class HtmlConverter
{
    private static readonly HashSet<string> ContentProperties = new()
    {
        "content", "html", "title", "value", "button_text", "quote_text", "speaker_name", "speaker_title", "heading",
        "richtext_field", "subheading", "price", "tab_label", "header", "subheader", "content_text", "alt", "text", "quotation",
        "author_name", "description", "speaker", "status", "event_time", "custom_cta_text", "short_description", "top_label"
    };

    private static readonly HashSet<string> ExcludeCustomModulesProperties = new()
    {
        "size_type", "loading", "src"
    };

    private static readonly HashSet<string> RawHtmlProperties = new()
    {
        "content", "html", "content_text", "richtext_field", "description", "short_description"
    };

    private const string OriginalContentAttribute = "original";
    private const string LanguageAttribute = "lang";
    private const string PathAttribute = "path";
    private const string BlackbirdReferenceIdAttribute = "blackbird-reference-id";
    private const string BlackbirdContentTypeAttribute = "blackbird-content-type";
    private const string BusinessUnitId = "business-unit-id";

    public static byte[] ToHtml(List<FieldGroupDto> fieldGroups, string title, string language, string pageId, string? translatedFromPageId,
        string contentType, string? editUrl)
    {
        var allFields = fieldGroups.SelectMany(group => group.Fields).ToList();
        var (doc, bodyNode) = PrepareEmptyHtmlDocument(new JObject(), title, language, pageId, translatedFromPageId, contentType, null, null, editUrl, null, null );

        foreach (var field in allFields)
        {
            if (field.Hidden)
                continue;

            var fieldDiv = doc.CreateElement("div");

            var jsonPath = $"fieldGroups.{field.Name}.html";
            fieldDiv.SetAttributeValue(PathAttribute, jsonPath);

            var fieldContentBuilder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(field.Label))
            {
                var label = HttpUtility.HtmlEncode(field.Label);
                fieldContentBuilder.AppendLine($"<label data-translate=\"label\">{label}</label>");
            }

            if (!string.IsNullOrWhiteSpace(field.Placeholder))
            {
                var placeholder = HttpUtility.HtmlEncode(field.Placeholder);
                fieldContentBuilder.AppendLine($"<span data-translate=\"placeholder\">{placeholder}</span>");
            }

            if (!string.IsNullOrWhiteSpace(field.Description))
            {
                var description = HttpUtility.HtmlEncode(field.Description);
                fieldContentBuilder.AppendLine($"<span data-translate=\"description\">{description}</span>");
            }

            if (field.FieldType == "dropdown" && field.Options != null)
            {
                fieldContentBuilder.AppendLine("<select>");

                foreach (var option in field.Options)
                {
                    if (!string.IsNullOrEmpty(option.Value))
                    {
                        fieldContentBuilder.AppendLine(
                            $"<option value=\"{HttpUtility.HtmlEncode(option.Value)}\">{HttpUtility.HtmlEncode(option.Label)}</option>");
                    }
                }

                fieldContentBuilder.AppendLine("</select>");
            }

            fieldDiv.InnerHtml = fieldContentBuilder.ToString();

            fieldDiv.SetAttributeValue("data-name", field.Name);
            fieldDiv.SetAttributeValue("data-field-type", field.FieldType);
            bodyNode.AppendChild(fieldDiv);
        }

        Console.WriteLine(doc.DocumentNode.OuterHtml);
        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    public static byte[] ToHtml(JObject emailContent, string title,string language, string pageId, string? translatedFromPageId, string contentType, LocalizablePropertiesRequest? properties, string slug, string? publicUrl, string? editUrl, string metaDescription, string subject, string? businessUnitId = null)
    {
        if (properties?.PropertiesToInclude != null)
        {
            foreach (var item in properties.PropertiesToInclude)
            { ContentProperties.Add(item); }
        }
        if (properties?.PropertiesToExclude != null)
        {
            foreach (var item in properties.PropertiesToExclude)
            {
                ContentProperties.Remove(item);
                ExcludeCustomModulesProperties.Add(item);
            }
        }

        var htmlNodes = emailContent.Descendants()
            .Where(x => x is JProperty { Value.Type: JTokenType.String } jProperty
                        && (
                            ContentProperties.Contains(jProperty.Name)
                            || (
                                jProperty.Ancestors()
                                    .OfType<JObject>()
                                    .Any(o => o["type"]?.Value<string>() == "custom_widget")
                                && jProperty.Path.Contains(".params.content.")
                                && !jProperty.Path.Contains(".params.content.quote_icon.")
                                && !ExcludeCustomModulesProperties.Contains(jProperty.Name)
                            )
                        )
            )
            .Select(x =>
            {
                var jProperty = (JProperty)x;
                return (
                    jProperty.Path,
                    Html: jProperty.Value.ToString()
                );
            })
            .ToList();

        var (doc, bodyNode) = PrepareEmptyHtmlDocument(emailContent, title, language, pageId, translatedFromPageId, contentType, slug, publicUrl, editUrl, metaDescription, subject, businessUnitId);
        htmlNodes.ForEach(x => AddContentToHtml(x.Path, x.Html, bodyNode, doc.CreateElement("div")));

        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    public static async Task<JsonResultEntity> ToJsonAsync(string targetLanguage, Stream htmlFile, UploadContentRequest uploadContentRequest, InvocationContext invocationContext)
    {
        if (uploadContentRequest.EnableInternalLinkLocalization == true)
        {
            var memoryStream = await CreateMemoryStreamFromFileAsync(htmlFile);
            
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(memoryStream);
            
            var links = htmlDoc.DocumentNode.SelectNodes("//a")?.ToList() ?? new List<HtmlNode>();
            memoryStream.Position = 0;
            
            await LocalizeLinksAsync(links, uploadContentRequest, targetLanguage, invocationContext);
            await LocalizeDivHrefsAsync(htmlDoc, uploadContentRequest, targetLanguage, invocationContext);

            if (links.Any())
            {
                memoryStream = SaveHtmlDocumentToStream(htmlDoc);
            }

            var (pageInfo, originalJson) = ToJson(memoryStream, uploadContentRequest.EnableInternalLinkLocalization ?? false, targetLanguage, invocationContext);
            pageInfo.Links = links;
            
            return new(pageInfo, originalJson);
        }
        else 
        {
            var (pageInfo, originalJson) = ToJson(htmlFile);
            return new(pageInfo, originalJson);
        }
    }

    public static (PageInfoResponse pageInfo, JObject json) ToJson(Stream htmlFile, bool updateOriginalJson = false, string? targetLanguage = null, InvocationContext? invocationContext = null)
    {
        var pageInfo = ExtractPageInfo(htmlFile);
        var doc = pageInfo.HtmlDocument;

        var bodyNode = doc.DocumentNode.SelectSingleNode("/html/body");

        var originalAttr = bodyNode.Attributes[OriginalContentAttribute];
        if (originalAttr == null)
        {
            throw new PluginMisconfigurationException($"The file is missing key atributes. Please check your input file and try again.");
        }

        var originalAttributeValue = HttpUtility.HtmlDecode(bodyNode.Attributes[OriginalContentAttribute].Value);
        var originalJson = JObjectExtensions.ToJObjectWithExceptionHandling(originalAttributeValue);
        if(updateOriginalJson && !string.IsNullOrEmpty(targetLanguage) && invocationContext != null)
        {
            originalJson = UpdateContentIds(originalJson, targetLanguage, invocationContext);
        }

        var contentNodes = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[PathAttribute] is not null)
            .ToList();

        contentNodes.ForEach(x =>
        {
            var path = x.Attributes[PathAttribute].Value;
            var token = originalJson.SelectToken(path)!;

            ((JValue)token).Value = x.InnerHtml;
        });

        return (pageInfo, originalJson);
    }

    private static JObject UpdateContentIds(JObject originalJson, string targetLanguage, InvocationContext invocationContext)
    {
        var updatedJson = (JObject)originalJson.DeepClone();
        var contentIdTokens = updatedJson.SelectTokens("$..url")
            .Where(t => t is JObject obj && obj["type"]?.ToString() == "CONTENT" && obj["content_id"] != null)
            .ToList();
        
        foreach (JObject urlToken in contentIdTokens)
        {
            var oldContentId = urlToken["content_id"]!.Value<string>();
            if(!string.IsNullOrEmpty(oldContentId))
            {
                ulong? newContentId = GetNewContentId(oldContentId, targetLanguage, invocationContext);
                if (newContentId.HasValue && newContentId.Value != 0)
                {
                    urlToken["content_id"] = newContentId;
                }
            }
        }
        
        return updatedJson;
    }

    private static ulong? GetNewContentId(string oldContentId, string targetLanguage, InvocationContext invocationContext)
    {
        try
        {
            var sitePageService = new SitePageService(invocationContext);
            var pageContent = sitePageService.GetPageAsync(oldContentId).GetAwaiter().GetResult();
            if (pageContent.Translations.TryGetValue(targetLanguage, out var pageTranslation))
            {
                return ulong.Parse(pageTranslation.Id);
            }
        }
        catch
        { }

        try
        {
            var blogPostService = new BlogPostService(invocationContext);
            var blogContent = blogPostService.GetBlogPostAsync(oldContentId).GetAwaiter().GetResult();
            if (blogContent.Translations.TryGetValue(targetLanguage, out var blogTranslation))
            {
                return ulong.Parse(blogTranslation.Id);
            }
        }
        catch
        { }

        return null;
    }

    public static string? ExtractBlackbirdId(byte[] fileBytes)
    {
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var doc = new HtmlDocument();
        doc.LoadHtml(fileString);
        var referenceId = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-reference-id']")
            ?.GetAttributeValue("content", null);

        return referenceId;
    }

    public static string? ExtractSubject(byte[] fileBytes)
    {
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var doc = new HtmlDocument();
        doc.LoadHtml(fileString);
        var subject = doc.DocumentNode.SelectSingleNode("//meta[@name='subject']")
            ?.GetAttributeValue("content", null);

        return subject;
    }

    public static string? ExtractBusinessUnitId(byte[] fileBytes)
    {
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var doc = new HtmlDocument();
        doc.LoadHtml(fileString);
        var referenceId = doc.DocumentNode.SelectSingleNode("//meta[@name='business-unit-id']")
            ?.GetAttributeValue("content", null);

        return referenceId;
    }

    public static string? ExtractTitle(byte[] fileBytes)
    {
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var doc = new HtmlDocument();
        doc.LoadHtml(fileString);
        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        return titleNode?.InnerText?.Trim();
    }

    public static string? ExtractLanguage(byte[] fileBytes)
    {
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var doc = new HtmlDocument();
        doc.LoadHtml(fileString);
        var language = doc.DocumentNode.SelectSingleNode("//body")
            ?.GetAttributeValue("lang", null);

        return language;
    }

    private static void AddContentToHtml(string path, string html, HtmlNode bodyNode, HtmlNode elementNode)
    {
        elementNode.InnerHtml = html;
        elementNode.SetAttributeValue(PathAttribute, path);

        bodyNode.AppendChild(elementNode);
    }

    private static void AddBlackbirdMeta(HtmlDocument htmlDoc, HtmlNode headNode, string name, string? value)
    {
        if (value is null) return;
        var entryMetaNode = htmlDoc.CreateElement("meta");
        entryMetaNode.SetAttributeValue("name", $"blackbird-{name}");
        entryMetaNode.SetAttributeValue("content", value);
        headNode.AppendChild(entryMetaNode);
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(JObject emailContent,
        string title, string language, string pageId, string? translatedFromPageId, string contentType, string? slug, string? publicUrl, string? editUrl, string? metaDescription, string? subject, string? businessUnitId = null)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlNode.SetAttributeValue("lang", language);
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        var sourcePageId = string.IsNullOrEmpty(translatedFromPageId) ? pageId : translatedFromPageId;

        if (!string.IsNullOrEmpty(title))
        {
            var titleNode = htmlDoc.CreateElement("title");
            titleNode.SetAttributeValue("data-blackbird-key", $"{sourcePageId}-title");
            headNode.AppendChild(titleNode);
            titleNode.InnerHtml = title;
        }

        if (!string.IsNullOrEmpty(pageId)) 
        {
            var metaNode = htmlDoc.CreateElement("meta");
            metaNode.SetAttributeValue("name", BlackbirdReferenceIdAttribute);
            metaNode.SetAttributeValue("content", pageId);
            headNode.AppendChild(metaNode);
        }

        var metaSlugNode = htmlDoc.CreateElement("meta");
        metaSlugNode.SetAttributeValue("name", "slug");
        metaSlugNode.SetAttributeValue("content", slug ?? string.Empty);
        headNode.AppendChild(metaSlugNode);

        if (!string.IsNullOrEmpty(metaDescription))
        {
            var metaDescriptionNode = htmlDoc.CreateElement("meta");
            metaDescriptionNode.SetAttributeValue("name", "description");
            metaDescriptionNode.SetAttributeValue("data-blackbird-key", $"{sourcePageId}-description");
            metaDescriptionNode.SetAttributeValue("content", metaDescription);
            headNode.AppendChild(metaDescriptionNode);
        }

        var contentTypeMetaNode = htmlDoc.CreateElement("meta");
        contentTypeMetaNode.SetAttributeValue("name", BlackbirdContentTypeAttribute);
        contentTypeMetaNode.SetAttributeValue("content", contentType);
        headNode.AppendChild(contentTypeMetaNode);

        if (!string.IsNullOrWhiteSpace(businessUnitId))
        {
            var metaBusinessUnitNode = htmlDoc.CreateElement("meta");
            metaBusinessUnitNode.SetAttributeValue("name", BusinessUnitId);
            metaBusinessUnitNode.SetAttributeValue("content", businessUnitId);
            headNode.AppendChild(metaBusinessUnitNode);
        }

        if (!string.IsNullOrWhiteSpace(subject))
        {
            var subjectNode = htmlDoc.CreateElement("meta");
            subjectNode.SetAttributeValue("name", "subject");
            subjectNode.SetAttributeValue("data-blackbird-key", $"{sourcePageId}-subject");
            subjectNode.SetAttributeValue("content", subject);
            headNode.AppendChild(subjectNode);
        }

        AddBlackbirdMeta(htmlDoc, headNode, "ucid", sourcePageId);
        AddBlackbirdMeta(htmlDoc, headNode, "content-name", title);
        AddBlackbirdMeta(htmlDoc, headNode, "admin-url", editUrl);
        AddBlackbirdMeta(htmlDoc, headNode, "public-url", publicUrl);
        AddBlackbirdMeta(htmlDoc, headNode, "system-name", "Hubspot");
        AddBlackbirdMeta(htmlDoc, headNode, "system-ref", "https://www.hubspot.com");

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        bodyNode.SetAttributeValue("its-rev-tool", "Hubspot");
        bodyNode.SetAttributeValue("its-rev-tool-ref", "https://www.hubspot.com");

        bodyNode.SetAttributeValue(OriginalContentAttribute, HttpUtility.HtmlEncode(emailContent.ToString()));
        bodyNode.SetAttributeValue(LanguageAttribute, language);

        return (htmlDoc, bodyNode);
    }

    public static PageInfoResponse ExtractPageInfo(Stream file)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        var bodyNode = doc.DocumentNode.SelectSingleNode("/html/body");
        if (bodyNode == null)
        {
            throw new PluginMisconfigurationException("Invalid HTML structure: missing body element. Please use a valid Hubspot HTML file.");
        }

        var originalAttr = bodyNode.Attributes[OriginalContentAttribute];
        if (originalAttr == null)
        {
            throw new PluginMisconfigurationException("The HTML file is missing required attributes. Please ensure you're using the file generated by 'Get marketing email content as HTML' action.");
        }

        return new()
        {
            HtmlDocument = doc,
            Title = doc.DocumentNode.SelectSingleNode("//title")?.InnerHtml ?? string.Empty,
            Language = bodyNode.Attributes[LanguageAttribute]?.Value
        };
    }

    public static FormHtmlEntities ExtractFormHtmlEntities(byte[] bytes)
    {
        var doc = new HtmlDocument();
        var memoryStream = new MemoryStream(bytes);
        doc.Load(memoryStream);

        var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerHtml ?? "";
        var properties = new List<FormHtmlEntity>();

        var divs = doc.DocumentNode.SelectNodes("//div[@data-name]");

        if (divs != null)
        {
            foreach (var div in divs)
            {
                // Retrieve the value of the data-name attribute
                var nameAttr = div.GetAttributeValue("data-name", null);
                if (string.IsNullOrEmpty(nameAttr))
                    continue; // Skip if data-name is missing

                var propertiesDict = new Dictionary<string, string>();

                // Find all child elements with a data-translate attribute
                var translatableElements = div.SelectNodes(".//*[@data-translate]");

                if (translatableElements != null)
                {
                    foreach (var elem in translatableElements)
                    {
                        var translateAttr = elem.GetAttributeValue("data-translate", null);
                        if (string.IsNullOrEmpty(translateAttr))
                            continue; // Skip if data-translate is missing

                        // Decode and trim the inner text
                        var text = HttpUtility.HtmlDecode(elem.InnerText.Trim());

                        // Add to properties dictionary
                        if (!propertiesDict.ContainsKey(translateAttr))
                        {
                            propertiesDict[translateAttr] = text;
                        }
                        else
                        {
                            // If the key already exists, append the text (for multiple entries like description)
                            propertiesDict[translateAttr] += $" {text}";
                        }
                    }
                }

                // Check if there's a select element to extract options
                var selectNode = div.SelectSingleNode(".//select");
                Dictionary<string, string>? optionsDict = null;

                if (selectNode != null)
                {
                    optionsDict = new Dictionary<string, string>();
                    var optionNodes = selectNode.SelectNodes(".//option");
                    if (optionNodes != null)
                    {
                        foreach (var option in optionNodes)
                        {
                            var value = option.GetAttributeValue("value", string.Empty);
                            var text = HttpUtility.HtmlDecode(option.InnerText.Trim());

                            if (!optionsDict.ContainsKey(value))
                            {
                                optionsDict[value] = text;
                            }
                            else
                            {
                                // Handle duplicate option values if necessary
                                // For now, we'll overwrite with the latest occurrence
                                optionsDict[value] = text;
                            }
                        }
                    }
                }
                
                var fieldType = div.GetAttributeValue("data-field-type", "single_line_text");
                var formHtmlEntity = new FormHtmlEntity(
                    Name: nameAttr,
                    Properties: propertiesDict,
                    Options: optionsDict,
                    FieldType: fieldType
                );

                properties.Add(formHtmlEntity);
            }
        }

        return new FormHtmlEntities(
            FormName: title,
            FieldGroups: properties
        );
    }

    private static async Task<MemoryStream> CreateMemoryStreamFromFileAsync(Stream file)
    {
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
    
    private static MemoryStream SaveHtmlDocumentToStream(HtmlDocument htmlDoc)
    {
        var memoryStream = new MemoryStream();
        htmlDoc.Save(memoryStream);
        memoryStream.Position = 0;
        return memoryStream;
    }
    
    public static async Task LocalizeLinksAsync(List<HtmlNode> links, UploadContentRequest uploadContentRequest, 
        string targetLanguage, InvocationContext invocationContext)
    {
        foreach (var link in links)
        {
            var href = link.GetAttributeValue("href", null);
            if (ShouldLocalizeLink(href, uploadContentRequest.PublishedSiteBaseUrl!))
            {
                await LocalizeLinkAsync(link, href, uploadContentRequest, targetLanguage, invocationContext);
            }
        }
    }
    
    private static bool ShouldLocalizeLink(string? href, string baseUrl)
    {
        if(string.IsNullOrEmpty(href) || string.IsNullOrEmpty(baseUrl))
        {
            return false;
        }

        if(href.StartsWith(baseUrl))
        {
            return true;
        }

        return !href.StartsWith("http", StringComparison.OrdinalIgnoreCase);
    }
    
    private static async Task LocalizeLinkAsync(HtmlNode link, string href, UploadContentRequest uploadContentRequest, 
        string targetLanguage, InvocationContext invocationContext)
    {
        if (string.IsNullOrEmpty(href) || string.IsNullOrEmpty(uploadContentRequest.PublishedSiteBaseUrl))
        {
            return;
        }

        try
        {
            string? fullUrl;
            if (href.StartsWith(uploadContentRequest.PublishedSiteBaseUrl))
            {
                fullUrl = href;
            }
            else
            {
                fullUrl = $"{uploadContentRequest.PublishedSiteBaseUrl!.TrimEnd('/')}{href}";
            }

            // Validate URL format before making request
            if (!Uri.TryCreate(fullUrl, UriKind.Absolute, out _))
            {
                return;
            }

            var htmlVariables = InternalUrlProvider.GetHtmlVariables(fullUrl);
            if (htmlVariables != null && htmlVariables.ChangeHref)
            {
                var pageId = htmlVariables.PageId;
                var pageType = htmlVariables.PageType;

                if (pageType.Equals("blog-post"))
                {
                    await LocalizeBlogPostLinkAsync(link, pageId, targetLanguage, invocationContext);
                }
                else if(pageType.Equals("standard-page")) 
                {
                    await LocalizeSitePageLinkAsync(link, pageId, targetLanguage, invocationContext);
                }
                else if (pageType.Equals("landing-page"))
                {
                    await LocalizeLandingPageLinkAsync(link, pageId, targetLanguage, invocationContext);
                }
            }
        }
        catch (Exception)
        {
            // Skip link localization if URL parsing or fetching fails
            // This is not a critical operation, so we continue processing
            return;
        }
    }
    
    private static async Task LocalizeBlogPostLinkAsync(HtmlNode link, string pageId, string targetLanguage, 
        InvocationContext invocationContext)
    {
        var blogPostService = new BlogPostService(invocationContext);
        var blogPost = await blogPostService.GetBlogPostAsync(pageId);
        
        if (blogPost.Translations.TryGetValue(targetLanguage, out var translation))
        {
            var translatedUrl = translation.Slug;
            link.SetAttributeValue("href", $"/{translatedUrl}");
        }
    }

    private static async Task LocalizeSitePageLinkAsync(HtmlNode link, string pageId, string targetLanguage, 
        InvocationContext invocationContext)
    {
        var sitePageService = new SitePageService(invocationContext);
        var sitePage = await sitePageService.GetPageAsync(pageId);
        
        if (sitePage.Translations.TryGetValue(targetLanguage, out var translation))
        {
            var translatedUrl = translation.Slug;
            link.SetAttributeValue("href", $"/{translatedUrl}");
        }
    }

    private static async Task LocalizeLandingPageLinkAsync(HtmlNode link, string pageId, string targetLanguage, 
        InvocationContext invocationContext)
    {
        var landingPageService = new LandingPageService(invocationContext);
        var landingPage = await landingPageService.GetLandingPageAsync(pageId);
        
        if (landingPage.Translations.TryGetValue(targetLanguage, out var translation))
        {
            var translatedUrl = translation.Slug;
            link.SetAttributeValue("href", $"/{translatedUrl}");
        }
    }

    private static async Task LocalizeDivHrefsAsync(HtmlDocument htmlDoc, UploadContentRequest uploadContentRequest, 
        string targetLanguage, InvocationContext invocationContext)
    {
        var divHrefs = htmlDoc.DocumentNode.SelectNodes("//div[contains(@path, 'href')]")?.ToList() ?? new List<HtmlNode>();
    
        foreach (var div in divHrefs)
        {
            string path = div.GetAttributeValue("path", "");
            if (!path.EndsWith(".href") && !path.EndsWith("href"))
                continue;
                
            string url = div.InnerHtml.Trim();
            if (ShouldLocalizeLink(url, uploadContentRequest.PublishedSiteBaseUrl!))
            {
                string? localizedUrl = await GetLocalizedUrlAsync(url, uploadContentRequest, targetLanguage, invocationContext);
                if (!string.IsNullOrEmpty(localizedUrl))
                {
                    div.InnerHtml = localizedUrl;
                }
            }
        }
    }

    private static async Task<string?> GetLocalizedUrlAsync(string url, UploadContentRequest uploadContentRequest, 
        string targetLanguage, InvocationContext invocationContext)
    {
        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(uploadContentRequest.PublishedSiteBaseUrl))
        {
            return null;
        }

        try
        {
            string? fullUrl;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                fullUrl = url;
            }
            else
            {
                fullUrl = $"{uploadContentRequest.PublishedSiteBaseUrl!.TrimEnd('/')}{url}";
            }

            // Validate URL format before making request
            if (!Uri.TryCreate(fullUrl, UriKind.Absolute, out _))
            {
                return null;
            }

            var htmlVariables = InternalUrlProvider.GetHtmlVariables(fullUrl);
            if (htmlVariables != null && htmlVariables.ChangeHref)
            {
                var pageId = htmlVariables.PageId;
                var pageType = htmlVariables.PageType;

                if (pageType.Equals("blog-post"))
                {
                    var blogPostService = new BlogPostService(invocationContext);
                    var blogPost = await blogPostService.GetBlogPostAsync(pageId);
                    
                    if (blogPost.Translations.TryGetValue(targetLanguage, out var translation))
                    {
                        return $"{uploadContentRequest.PublishedSiteBaseUrl!.TrimEnd('/')}/{translation.Slug}";
                    }
                }
                else if(pageType.Equals("standard-page")) 
                {
                    var sitePageService = new SitePageService(invocationContext);
                    var sitePage = await sitePageService.GetPageAsync(pageId);
                    
                    if (sitePage.Translations.TryGetValue(targetLanguage, out var translation))
                    {
                        return $"{uploadContentRequest.PublishedSiteBaseUrl!.TrimEnd('/')}/{translation.Slug}";
                    }
                }
                else if (pageType.Equals("landing-page"))
                {
                    var landingPageService = new LandingPageService(invocationContext);
                    var landingPage = await landingPageService.GetLandingPageAsync(pageId);
                    
                    if (landingPage.Translations.TryGetValue(targetLanguage, out var translation))
                    {
                        return $"{uploadContentRequest.PublishedSiteBaseUrl!.TrimEnd('/')}/{translation.Slug}";
                    }
                }
            }
        }
        catch (Exception)
        {
            // Skip link localization if URL parsing or fetching fails
            // This is not a critical operation, so we continue processing
            return null;
        }
        
        return null;
    }
}