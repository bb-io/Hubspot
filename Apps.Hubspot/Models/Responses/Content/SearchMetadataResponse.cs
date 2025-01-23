using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Content;

public class SearchMetadataResponse
{
    [Display("Metadata")]
    public List<Metadata> Metadata { get; set; } = new();
    
    public SearchMetadataResponse()
    { }
    
    public SearchMetadataResponse(List<Metadata> metadata)
    {
        Metadata = metadata;
    }
}