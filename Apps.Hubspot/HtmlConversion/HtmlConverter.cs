using System.Text;
using System.Web;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Responses.Pages;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.HtmlConversion;

public static class HtmlConverter
{
    private static readonly HashSet<string> ContentProperties = new HashSet<string>
    {
        "content", "html", "title", "value", "button_text", "quote_text", "speaker_name", "speaker_title", "heading", "richtext_field",
        "subheading", "price", "tab_label", "header", "subheader", "content_text", "alt", "text", "quotation",
        "author_name",
        "description"
    };

    private static readonly HashSet<string> RawHtmlProperties = new HashSet<string>
    {
        "content", "html", "content_text", "richtext_field"
    };

    private const string OriginalContentAttribute = "original";
    private const string LanguageAttribute = "lang";
    private const string PathAttribute = "path";
    private const string BlackbirdReferenceIdAttribute = "blackbird-reference-id";
    private const string BusinessUnitId = "business-unit-id";

    public static byte[] ToHtml(List<FieldGroupDto> fieldGroups, string title, string language, string pageId)
    {
        var allFields = fieldGroups.SelectMany(group => group.Fields).ToList();
        var (doc, bodyNode) = PrepareEmptyHtmlDocument(new JObject(), title, language, pageId);

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
            bodyNode.AppendChild(fieldDiv);
        }

        Console.WriteLine(doc.DocumentNode.OuterHtml);
        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    public static byte[] ToHtml(JObject emailContent, string title, string language, string pageId, string contentType, string? businessUnitId = null)
    {
        var htmlNodes = emailContent.Descendants()
            .Where(x => x is JProperty { Value.Type: JTokenType.String } jProperty &&
                        ContentProperties.Contains(jProperty.Name))
            .Select(x =>
            {
                var jProperty = x as JProperty;
                return (jProperty!.Path,
                    Html: RawHtmlProperties.Contains(jProperty.Name)
                        ? jProperty.Value.ToString()
                        : HttpUtility.HtmlEncode(jProperty.Value.ToString()));
            })
            .ToList();

        var (doc, bodyNode) = PrepareEmptyHtmlDocument(emailContent, title, language, pageId, businessUnitId);

        htmlNodes.ForEach(x => AddContentToHtml(x.Path, x.Html, bodyNode, doc.CreateElement("div")));

        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    public static (PageInfoResponse pageInfo, JObject json) ToJson(Stream htmlFile)
    {
        var pageInfo = ExtractPageInfo(htmlFile);
        var doc = pageInfo.HtmlDocument;

        var bodyNode = doc.DocumentNode.SelectSingleNode("/html/body");

        var originalAttributeValue = HttpUtility.HtmlDecode(bodyNode.Attributes[OriginalContentAttribute].Value);
        var originalJson = JObject.Parse(originalAttributeValue);

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

    public static string? ExtractBlackbirdId(byte[] fileBytes)
    {
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var doc = new HtmlDocument();
        doc.LoadHtml(fileString);
        var referenceId = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-reference-id']")
            ?.GetAttributeValue("content", null);

        return referenceId;
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

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(JObject emailContent,
        string title, string language, string pageId, string? businessUnitId=null)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        var titleNode = htmlDoc.CreateElement("title");
        headNode.AppendChild(titleNode);
        titleNode.InnerHtml = title;

        var metaNode = htmlDoc.CreateElement("meta");
        metaNode.SetAttributeValue("name", BlackbirdReferenceIdAttribute);
        metaNode.SetAttributeValue("content", pageId);
        headNode.AppendChild(metaNode);

        if (!string.IsNullOrWhiteSpace(businessUnitId))
        {
            var metaBusinessUnitNode = htmlDoc.CreateElement("meta");
            metaBusinessUnitNode.SetAttributeValue("name", BusinessUnitId);
            metaBusinessUnitNode.SetAttributeValue("content", businessUnitId);
            headNode.AppendChild(metaBusinessUnitNode);
        }

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        bodyNode.SetAttributeValue(OriginalContentAttribute, HttpUtility.HtmlEncode(emailContent.ToString()));
        bodyNode.SetAttributeValue(LanguageAttribute, language);

        return (htmlDoc, bodyNode);
    }

    public static PageInfoResponse ExtractPageInfo(Stream file)
    {
        var doc = new HtmlDocument();
        doc.Load(file);

        return new()
        {
            HtmlDocument = doc,
            Title = doc.DocumentNode.SelectSingleNode("//title").InnerHtml,
            Language = doc.DocumentNode.SelectSingleNode("/html/body").Attributes[LanguageAttribute]?.Value
        };
    }

    public static FormHtmlEntities ExtractFormHtmlEntities(byte[] bytes)
    {
        var doc = new HtmlDocument();
        var memoryStream = new MemoryStream(bytes);
        doc.Load(memoryStream);

        var title = doc.DocumentNode.SelectSingleNode("//title").InnerHtml;
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

                // Create a FormHtmlEntity with the extracted data
                var formHtmlEntity = new FormHtmlEntity(
                    Name: nameAttr,
                    Properties: propertiesDict,
                    Options: optionsDict
                );

                properties.Add(formHtmlEntity);
            }
        }

        return new FormHtmlEntities(
            FormName: title,
            FieldGroups: properties
        );
    }
}