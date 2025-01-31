using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Content;

public class Metadata
{
    [Display("Content ID")]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    
    [Display("Content type")]
    public string Type { get; set; } = string.Empty;

    [Display("Language")]
    public string Language { get; set; } = string.Empty;

    [Display("Published")]
    public bool Published { get; set; }
    
    public string State { get; set; } = string.Empty;

    [Display("Created at")]
    public DateTime CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }
}
