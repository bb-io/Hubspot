using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Emails;

public class MarketingEmailDto
{
    [Display("Marketing email ID")]
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string Subject { get; set; } = string.Empty;
    
    [Display("Publish at")]
    public DateTime? PublishedAt { get; set; }
    
    [Display("Updated at")]
    public DateTime? UpdatedAt { get; set; }
    
    [Display("Created at")]
    public DateTime CreatedAt { get; set; }
    
    public string Language { get; set; } = string.Empty;
    
    public string State { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty;
    
    [Display("Is published")]
    public bool IsPublished { get; set; }
    
    public bool Archived { get; set; }

    [Display("Active domain")]
    public string ActiveDomain { get; set; } = string.Empty;
}