using HtmlAgilityPack;

namespace Apps.Hubspot.Extensions;

public static class HtmlDocumentExtension
{
    public static string? ExtractBlackbirdReferenceId(this HtmlDocument doc)
    {
        var metaNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-reference-id']");
        return metaNode.GetAttributeValue("content", null);
    }
}