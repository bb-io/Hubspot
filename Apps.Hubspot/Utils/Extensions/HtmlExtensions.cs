using System.Web;
using Apps.Hubspot.Models.Dtos.Emails;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Requests.Forms;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Hubspot.Utils.Extensions;

public static class HtmlExtensions
{
    private const string BlackbirdReferenceIdAttribute = "blackbird-reference-id";

    public static string AsHtml(this (string title, string metaDescription, string body, string pageId) tuple)
    {
        return
            $"<html><head><title>{tuple.title}</title><meta name=\"{BlackbirdReferenceIdAttribute}\" content=\"{tuple.pageId}\"><description>{tuple.metaDescription}</description></head><body>{tuple.body}</body></html>";
    }

    public static string GetNodeFromHead(this HtmlDocument doc, string nodeName)
    {
        return GetHtmlText(doc, $"html/head/{nodeName}");
    }
    
    private static string GetHtmlText(HtmlDocument doc, string xPath)
    {
        return HttpUtility.HtmlDecode(doc.DocumentNode.SelectSingleNode(xPath).InnerText);
    }

    public static HtmlEmailValues ExtractHtmlValuesForEmail(HtmlDocument htmlDoc)
    {
        var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
        var title = titleNode?.InnerText.Trim()
            ?? throw new PluginApplicationException("The HTML file does not contain a valid title.");

        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
        if (bodyNode == null)
        {
            throw new PluginApplicationException("The HTML file does not contain a valid body section.");
        }

        var nameNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='name']");
        var subjectNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='subject']");
        var sendOnPublishNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='sendOnPublish']");
        var archivedNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='archived']");
        var activeDomainNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='activeDomain']");
        var languageNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='language']");
        var publishDateNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='publishDate']");
        var businessUnitIdNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='businessUnitId']");

        return new HtmlEmailValues
        {
            Title = title,
            Body = bodyNode.InnerHtml,
            Name = nameNode?.InnerText.Trim(),
            Subject = subjectNode?.InnerText.Trim(),
            SendOnPublish = sendOnPublishNode != null && bool.TryParse(sendOnPublishNode.InnerText.Trim(), out var sendOnPublish) ? sendOnPublish : (bool?)null,
            Archived = archivedNode != null && bool.TryParse(archivedNode.InnerText.Trim(), out var archived) ? archived : (bool?)null,
            ActiveDomain = activeDomainNode?.InnerText.Trim(),
            Language = languageNode?.InnerText.Trim(),
            PublishDate = publishDateNode != null && DateTime.TryParse(publishDateNode.InnerText.Trim(), out var publishDate) ? publishDate : (DateTime?)null,
            BusinessUnitId = businessUnitIdNode?.InnerText.Trim()
        };
    }


    public static HtmlFormValues ExtractHtmlValuesForForm(HtmlDocument htmlDoc)
    {
        var nameMetaNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='name']");
        var nameNode = nameMetaNode?.GetAttributeValue("content", null)
                       ?? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='name']")?.InnerText.Trim();
        var formName = nameNode ?? "Default Form Name";

        var typeMetaNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='type']");
        var typeNode = typeMetaNode?.GetAttributeValue("content", null)
                       ?? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='type']")?.InnerText.Trim();
        var formType = typeNode ?? "hubspot";

        var languageMetaNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='language']");
        var languageNode = languageMetaNode?.GetAttributeValue("content", null)
                           ?? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='language']")?.InnerText.Trim();
        var language = languageNode ?? "en";

        var archivedMetaNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='archived']");
        var archivedNode = archivedMetaNode?.GetAttributeValue("content", null)?.ToLower()
                           ?? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='archived']")?.InnerText.Trim().ToLower();
        var archived = archivedNode == "true";

        var createMarketingFormRequest = new HtmlFormValues
        {
            Name = formName,
            FormType = formType,
            Language = language,
            Archived = archived
        };

        return createMarketingFormRequest;
    }
}