using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.Content;

public class UploadContentRequest
{
    [Display("Enable internal link localization")]
    public bool? EnableInternalLinkLocalization { get; set; }

    [Display("Published site base URL", Description = "The base URL of the published site. Required for internal link localization. This is used to determine the correct URL for internal links.")]
    public string? PublishedSiteBaseUrl { get; set; }
}
