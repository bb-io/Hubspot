using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Tags;

public class BlogTagDto : IEntity
{
    [Display("Blog tag ID")] 
    public string Id { get; set; } = string.Empty;
    
    [Display("Name")] 
    public string Name { get; set; } = string.Empty;
    
    [Display("Slug")] 
    public string Slug { get; set; } = string.Empty;
    
    [Display("Language")] 
    public string Language { get; set; } = string.Empty;
    
    [Display("Translated from ID")] 
    public string? TranslatedFromId { get; set; }
    
    [Display("Created")] 
    public string Created { get; set; } = string.Empty;
    
    [Display("Updated")] 
    public string Updated { get; set; } = string.Empty;
    
    [Display("Deleted at")] 
    public string DeletedAt { get; set; } = string.Empty;
}
