using System.Web;
using HtmlAgilityPack;

namespace Apps.Hubspot.Utils.Extensions;

public static class HtmlExtensions
{
    public static string AsHtml(this (string title, string metaDescription, string body) tuple)
    {
        return
            $"<html><head><title>{tuple.title}</title><description>{tuple.metaDescription}</description></head><body>{tuple.body}</body></html>";
    }
    
    public static string GetNodeFromHead(this HtmlDocument doc, string nodeName)
    {
        return GetHtmlText(doc, $"html/head/{nodeName}");
    }
    
    private static string GetHtmlText(HtmlDocument doc, string xPath)
    {
        return HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode(xPath).InnerText);
    }
}