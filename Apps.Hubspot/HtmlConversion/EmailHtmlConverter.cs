using System.Text;
using System.Web;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.HtmlConversion;

public static class EmailHtmlConverter
{
    private const string HtmlPropertyName = "html";
    private const string OriginalContentAttribute = "original";
    private const string PathAttribute = "path";
    private const string RedundantPathPrefix = "content.";
    
    public static byte[] ToHtml(JObject emailContent)
    {
        var htmlNodes = emailContent.Descendants()
            .Where(x => x is JProperty jProperty && jProperty.Name == HtmlPropertyName)
            .Select(x =>
            {
                var jProperty = x as JProperty;
                return (jProperty!.Path, Html: jProperty.Value.ToString());
            })
            .ToList();

        var (doc, bodyNode) = PrepareEmptyHtmlDocument(emailContent);

        htmlNodes.ForEach(x => AddContentToHtml(x.Path, x.Html, bodyNode, doc.CreateElement("div")));
        
        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    public static JObject ToJson(Stream htmlFile)
    {
        var doc = new HtmlDocument();
        doc.Load(htmlFile);

        var bodyNode = doc.DocumentNode.SelectSingleNode("/html/body");

        var originalAttributeValue = HttpUtility.HtmlDecode(bodyNode.Attributes[OriginalContentAttribute].Value);
        var originalJson = JObject.Parse(originalAttributeValue);

        var contentNodes = doc.DocumentNode.Descendants()
            .Where(x => x.Attributes[PathAttribute] is not null)
            .ToList();
        
        contentNodes.ForEach(x =>
        {
            var path = x.Attributes[PathAttribute].Value.Substring(RedundantPathPrefix.Length);
            var token = originalJson.SelectToken(path)!;

            ((JValue)token).Value = x.InnerHtml;
        });

        return originalJson;
    }
    
    private static void AddContentToHtml(string path, string html, HtmlNode bodyNode, HtmlNode elementNode)
    {
        elementNode.InnerHtml = html;
        elementNode.SetAttributeValue(PathAttribute, path);
        
        bodyNode.AppendChild(elementNode);
    }
    
    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(JObject emailContent)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);
        htmlNode.AppendChild(htmlDoc.CreateElement("head"));

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        bodyNode.SetAttributeValue(OriginalContentAttribute, emailContent.ToString());

        return (htmlDoc, bodyNode);
    }
}