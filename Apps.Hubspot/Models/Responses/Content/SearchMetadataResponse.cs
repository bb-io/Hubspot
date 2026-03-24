using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Hubspot.Models.Responses.Content;

public class SearchMetadataResponse(List<MetadataPollingDto> metadata) : IMultiDownloadableContentOutput<MetadataPollingDto>
{
    [Display("Metadata")]
    public List<MetadataPollingDto> Items { get; set; } = metadata;
}