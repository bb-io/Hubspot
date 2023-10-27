using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Posts;

public class TranslationDto
{
    [Display("Archived in dashboard")]
    public bool ArchivedInDashboard { get; set; }
    
    [Display("Creation date")]
    public DateTime Created { get; set; }
    
    [Display("Translation ID")]
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    [Display("Public access rules enabled")]
    public bool PublicAccessRulesEnabled { get; set; }
    
    [Display("Publish date")]
    public DateTime PublishDate { get; set; }
    
    public string Slug { get; set; }
    
    public string State { get; set; }
    
    [Display("Tag IDs")]
    public IEnumerable<string> TagIds { get; set; }
    
    [Display("Update date")]
    public DateTime Updated { get; set; }
}