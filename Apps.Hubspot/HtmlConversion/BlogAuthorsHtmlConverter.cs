using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Blogs.Authors;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Web;

namespace Apps.Hubspot.HtmlConversion;

public static class BlogAuthorsHtmlConverter
{
    private const string BlackbirdReferenceIdAttribute = "blackbird-reference-id";
    private const string BlackbirdContentTypeAttribute = "blackbird-content-type";

    public static string ConvertToHtml(BlogAuthorDto blogAuthor)
    {
        // Create HTML document
        var doc = new HtmlDocument();

        // Create HTML structure
        var htmlNode = doc.CreateElement("html");
        doc.DocumentNode.AppendChild(htmlNode);

        // Create head section
        var headNode = doc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        // Add meta tags
        AddMetaTag(doc, headNode, "charset", "UTF-8");
        AddMetaTag(doc, headNode, BlackbirdReferenceIdAttribute, blogAuthor.Id);
        AddMetaTag(doc, headNode, BlackbirdContentTypeAttribute, ContentTypes.BlogAuthor);
        AddMetaTag(doc, headNode, "slug", blogAuthor.Slug);

        // Create body with original JSON
        var bodyNode = doc.CreateElement("body");
        bodyNode.SetAttributeValue("original", HttpUtility.HtmlEncode(JsonConvert.SerializeObject(blogAuthor)));
        htmlNode.AppendChild(bodyNode);

        // Add localizable fields to body
        AddLocalizableField(doc, bodyNode, "bio", blogAuthor.Bio);
        AddLocalizableField(doc, bodyNode, "displayName", blogAuthor.DisplayName);
        AddLocalizableField(doc, bodyNode, "fullName", blogAuthor.FullName);
        AddLocalizableField(doc, bodyNode, "name", blogAuthor.Name);

        return "<!DOCTYPE html>\n" + doc.DocumentNode.OuterHtml;
    }

    public static BlogAuthorDto ConvertFromHtml(string htmlContent)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        var bodyNode = doc.DocumentNode.SelectSingleNode("//body");
        if (bodyNode == null)
        {
            throw new InvalidOperationException("No <body> node found in HTML.");
        }

        var originalJson = bodyNode.GetAttributeValue("original", null);
        if (string.IsNullOrEmpty(originalJson))
        {
            throw new InvalidOperationException("No 'original' attribute found on <body> node.");
        }

        var blogAuthor = JsonConvert.DeserializeObject<BlogAuthorDto>(HttpUtility.HtmlDecode(originalJson));
        if (blogAuthor == null)
        {
            throw new InvalidOperationException("Failed to deserialize BlogAuthorDto from original JSON.");
        }

        var slugMeta = doc.DocumentNode.SelectSingleNode("//meta[@name='slug']");
        if (slugMeta != null)
        {
            var slugValue = slugMeta.GetAttributeValue("content", null);
            if (!string.IsNullOrEmpty(slugValue))
            {
                blogAuthor.Slug = slugValue;
            }
        }

        void UpdateLocalizableField(string propertyName, Action<string> setValue)
        {
            var node = bodyNode.SelectSingleNode($".//div[@class='property-value' and @data-json-path='{propertyName}']");
            if (node != null)
            {
                setValue(node.InnerText);
            }
        }

        UpdateLocalizableField("bio", v => blogAuthor.Bio = v);
        UpdateLocalizableField("displayName", v => blogAuthor.DisplayName = v);
        UpdateLocalizableField("fullName", v => blogAuthor.FullName = v);
        UpdateLocalizableField("name", v => blogAuthor.Name = v);

        return blogAuthor;
    }

    private static void AddMetaTag(HtmlDocument doc, HtmlNode parentNode, string name, string content)
    {
        var metaTag = doc.CreateElement("meta");

        if (name == "charset")
        {
            metaTag.SetAttributeValue(name, content);
        }
        else
        {
            metaTag.SetAttributeValue("name", name);
            metaTag.SetAttributeValue("content", content);
        }

        parentNode.AppendChild(metaTag);
    }

    private static void AddLocalizableField(HtmlDocument doc, HtmlNode parentNode, string fieldName, string fieldValue)
    {
        var container = doc.CreateElement("div");
        container.SetAttributeValue("class", "property-value");
        container.SetAttributeValue("data-json-path", fieldName);

        var valueNode = doc.CreateTextNode(fieldValue);
        container.AppendChild(valueNode);

        parentNode.AppendChild(container);
    }
}
