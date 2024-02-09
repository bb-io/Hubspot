using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Emails;

public class MarketingEmailDto
{
    [Display("Marketing email ID")]
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string Subject { get; set; }
    
    [Display("Publish at")]
    public DateTime? PublishedAt { get; set; }
    
    [Display("Updated at")]
    public DateTime? UpdatedAt { get; set; }
    
    [Display("Created at")]
    public DateTime CreatedAt { get; set; }
    
    public string Language { get; set; }
    
    public string Type { get; set; }
    
    [Display("Is published")]
    public bool IsPublished { get; set; }
    
    public bool Archived { get; set; }
}