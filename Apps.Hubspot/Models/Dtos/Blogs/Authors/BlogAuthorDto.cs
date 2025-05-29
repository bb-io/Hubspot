using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Authors;

public class BlogAuthorDto : IEntity
{
    [Display("Blog author ID")] 
    public string Id { get; set; } = string.Empty;
    
    [Display("Name")] 
    public string Name { get; set; } = string.Empty;
    
    [Display("Full name")] 
    public string FullName { get; set; } = string.Empty;
    
    [Display("Display name")] 
    public string DisplayName { get; set; } = string.Empty;
    
    [Display("Slug")] 
    public string Slug { get; set; } = string.Empty;
    
    [Display("Bio")] 
    public string Bio { get; set; } = string.Empty;
    
    [Display("Email")] 
    public string Email { get; set; } = string.Empty;
    
    [Display("Website")] 
    public string Website { get; set; } = string.Empty;
    
    [Display("Avatar")] 
    public string Avatar { get; set; } = string.Empty;
    
    [Display("Facebook")] 
    public string Facebook { get; set; } = string.Empty;
    
    [Display("Twitter")] 
    public string Twitter { get; set; } = string.Empty;
    
    [Display("LinkedIn")] 
    public string Linkedin { get; set; } = string.Empty;
    
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
