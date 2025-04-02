using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Content;

public class SearchMetadataResponse
{
    [Display("Metadata")]
    public List<MetadataPollingDto> Metadata { get; set; } = new();
    
    public SearchMetadataResponse()
    { }
    
    public SearchMetadataResponse(List<MetadataPollingDto> metadata)
    {
        Metadata = metadata;
    }
}