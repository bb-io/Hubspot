using Blackbird.Applications.Sdk.Common;
using static Apps.Hubspot.Actions.Content.MetaActions;

namespace Apps.Hubspot.Models.Responses;
public class FindContentTypeResponse
{
    [Display("Content type")]
    public HubSpotContentType? ContentType { get; set; }

    [Display("Found")]
    public bool Found => ContentType.HasValue;
}