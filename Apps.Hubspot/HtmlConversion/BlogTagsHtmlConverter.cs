using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Blogs.Tags;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Web;

namespace Apps.Hubspot.HtmlConversion;

public static class BlogTagsHtmlConverter
{
    private const string BlackbirdReferenceIdAttribute = "blackbird-reference-id";
    private const string BlackbirdContentTypeAttribute = "blackbird-content-type";

    public static string ConvertToHtml(BlogTagDto blogTag)
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
        AddMetaTag(doc, headNode, BlackbirdReferenceIdAttribute, blogTag.Id);
        AddMetaTag(doc, headNode, BlackbirdContentTypeAttribute, ContentTypes.BlogTag);
        AddMetaTag(doc, headNode, "slug", blogTag.Slug);

        // Create body with original JSON
        var bodyNode = doc.CreateElement("body");
        bodyNode.SetAttributeValue("original", HttpUtility.HtmlEncode(JsonConvert.SerializeObject(blogTag)));
        htmlNode.AppendChild(bodyNode);

        // Add localizable fields to body - for blog tags, only the name is localizable
        AddLocalizableField(doc, bodyNode, "name", blogTag.Name);

        return "<!DOCTYPE html>\n" + doc.DocumentNode.OuterHtml;
    }

    public static BlogTagDto? ConvertFromHtml(string htmlContent)
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

        var blogTag = JsonConvert.DeserializeObject<BlogTagDto>(HttpUtility.HtmlDecode(originalJson));
        if (blogTag == null)
        {
            throw new InvalidOperationException("Failed to deserialize BlogTagDto from original JSON.");
        }

        var slugMeta = doc.DocumentNode.SelectSingleNode("//meta[@name='slug']");
        if (slugMeta != null)
        {
            var slugValue = slugMeta.GetAttributeValue("content", null);
            if (!string.IsNullOrEmpty(slugValue))
            {
                blogTag.Slug = slugValue;
            }
        }

        // Update localizable fields from the body
        var nameNode = bodyNode.SelectSingleNode(".//div[@class='property-value' and @data-json-path='name']");
        if (nameNode != null)
        {
            blogTag.Name = nameNode.InnerText;
        }

        return blogTag;
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
