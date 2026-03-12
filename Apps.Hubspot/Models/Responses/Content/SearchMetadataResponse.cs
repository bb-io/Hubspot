using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Content;

public class SearchMetadataResponse
{
    [Display("Metadata")]
    public List<MetadataPollingDto> Items { get; set; } = new();
    
    public SearchMetadataResponse()
    { }
    
    public SearchMetadataResponse(List<MetadataPollingDto> metadata)
    {
        Items = metadata;
    }
}