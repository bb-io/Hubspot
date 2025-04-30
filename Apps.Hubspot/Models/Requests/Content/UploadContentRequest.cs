using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.Content;

public class UploadContentRequest
{
    [Display("Enable internal link localization")]
    public bool? EnableInternalLinkLocalization { get; set; }

    [Display("Published site base URL", Description = "The base URL of the published site. Required for internal link localization. This is used to determine the correct URL for internal links.")]
    public string? PublishedSiteBaseUrl { get; set; }

    [Display("Create new content", Description = "If set to true for emails and forms, instead of updating the original item, it will create a new one. Default is false.")]
    public bool? CreateNew { get; set; }

    [Display("Original item ID", Description = "ID of blog post, landing or site page you want to update.")]
    public string? SitePageId { get; set; }

    [Display("Update slug and meta description from file", Description = "Page metadata will be updated from the meta tags in the file.")]
    public bool? UpdatePageMetdata { get; set; }

}