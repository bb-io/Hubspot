using System.Web;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Blogs.Comments;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Apps.Hubspot.HtmlConversion;

public static class BlogCommentsHtmlConverter
{
    private const string BlackbirdReferenceIdAttribute = "blackbird-reference-id";
    private const string BlackbirdContentTypeAttribute = "blackbird-content-type";

    public static string ConvertToHtml(BlogCommentDto blogComment)
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
        AddMetaTag(doc, headNode, BlackbirdReferenceIdAttribute, blogComment.Id);
        AddMetaTag(doc, headNode, BlackbirdContentTypeAttribute, ContentTypes.BlogComment);

        // Create body with original JSON
        var bodyNode = doc.CreateElement("body");
        bodyNode.SetAttributeValue("original", HttpUtility.HtmlEncode(JsonConvert.SerializeObject(blogComment)));
        htmlNode.AppendChild(bodyNode);

        // Add localizable fields to body
        AddLocalizableField(doc, bodyNode, "contentTitle", blogComment.ContentTitle);
        AddLocalizableField(doc, bodyNode, "comment", blogComment.Comment);

        return "<!DOCTYPE html>\n" + doc.DocumentNode.OuterHtml;
    }

    public static BlogCommentDto ConvertFromHtml(string htmlContent)
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

        var blogComment = JsonConvert.DeserializeObject<BlogCommentDto>(HttpUtility.HtmlDecode(originalJson));
        if (blogComment == null)
        {
            throw new InvalidOperationException("Failed to deserialize BlogCommentDto from original JSON.");
        }

        void UpdateLocalizableField(string propertyName, Action<string> setValue)
        {
            var node = bodyNode.SelectSingleNode($".//div[@class='property-value' and @data-json-path='{propertyName}']");
            if (node != null)
            {
                setValue(node.InnerText);
            }
        }

        UpdateLocalizableField("contentTitle", v => blogComment.ContentTitle = v);
        UpdateLocalizableField("comment", v => blogComment.Comment = v);

        return blogComment;
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
        container.InnerHtml = fieldValue ?? string.Empty;
        parentNode.AppendChild(container);
    }
}
