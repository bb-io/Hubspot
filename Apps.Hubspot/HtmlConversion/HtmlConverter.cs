using System.Text;
using System.Web;
using Apps.Hubspot.Models.Responses.Pages;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.HtmlConversion;

public static class HtmlConverter
{
    private static readonly HashSet<string> ContentProperties =
    [
        "content", "html", "title", "value", "button_text", "quote_text", "speaker_name", "speaker_title", "heading",
        "subheading", "price", "tab_label", "header", "subheader", "content_text", "alt", "text", "quotation", "author_name",
        "description"
    ];

    private static readonly HashSet<string> RawHtmlProperties = 
    [
        "content", "html", "content_text"
    ];

    private const string OriginalContentAttribute = "original";
    private const string LanguageAttribute = "lang";
    private const string PathAttribute = "path";

    public static byte[] ToHtml(JObject emailContent, string title, string language)
    {
        var htmlNodes = emailContent.Descendants()
            .Where(x => x is JProperty { Value.Type: JTokenType.String } jProperty &&
                        ContentProperties.Contains(jProperty.Name))
            .Select(x =>
            {
                var jProperty = x as JProperty;
                return (jProperty!.Path, Html: RawHtmlProperties.Contains(jProperty.Name) ? jProperty.Value.ToString() : HttpUtility.HtmlEncode(jProperty.Value.ToString()) );
            })
            .ToList();

        var (doc, bodyNode) = PrepareEmptyHtmlDocument(emailContent, title, language);

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

    private static void AddContentToHtml(string path, string html, HtmlNode bodyNode, HtmlNode elementNode)
    {
        elementNode.InnerHtml = html;
        elementNode.SetAttributeValue(PathAttribute, path);

        bodyNode.AppendChild(elementNode);
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(JObject emailContent,
        string title, string language)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        var headNode = htmlDoc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        var titleNode = htmlDoc.CreateElement("title");
        headNode.AppendChild(titleNode);
        titleNode.InnerHtml = title;

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        bodyNode.SetAttributeValue(OriginalContentAttribute, HttpUtility.HtmlEncode(emailContent.ToString()));
        bodyNode.SetAttributeValue(LanguageAttribute, language);

        return (htmlDoc, bodyNode);
    }

    private static PageInfoResponse ExtractPageInfo(Stream file)
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
}