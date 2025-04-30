using HtmlAgilityPack;

namespace Apps.Hubspot.Extensions;

public static class HtmlDocumentExtension
{
    public static string? ExtractBlackbirdReferenceId(this HtmlDocument doc)
    {
        var metaNode = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-reference-id']");
        return metaNode.GetAttributeValue("content", null);
    }
    public static string? ExtractSlug(this HtmlDocument doc)
    {
        var metaNode = doc.DocumentNode.SelectSingleNode("//meta[@name='slug']");
        return metaNode?.GetAttributeValue("content", null);
    }
    public static string? ExtractMetaDescription(this HtmlDocument doc)
    {
        var metaNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
        return metaNode?.GetAttributeValue("content", null);
    }
}