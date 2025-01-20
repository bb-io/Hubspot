using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Content;

public class Metadata
{
    [Display("Content ID")]
    public string Id { get; set; }
    
    [Display("Content type")]
    public string Type { get; set; }

    [Display("Language")]
    public string Language { get; set; }
}
