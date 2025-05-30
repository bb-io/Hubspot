using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Comments;

public class BlogCommentDto
{
    [Display("Comment ID")] 
    public string Id { get; set; } = string.Empty;
    
    [Display("Portal ID")] 
    public long PortalId { get; set; }
    
    [Display("Content ID")] 
    public long ContentId { get; set; }
    
    [Display("Content title")] 
    public string ContentTitle { get; set; } = string.Empty;
    
    [Display("Content permalink")] 
    public string? ContentPermalink { get; set; }
    
    [Display("Collection ID")] 
    public long CollectionId { get; set; }
    
    [Display("Created at")] 
    public long CreatedAt { get; set; }
    
    [Display("Deleted at")] 
    public long DeletedAt { get; set; }
    
    [Display("User name")] 
    public string UserName { get; set; } = string.Empty;
    
    [Display("First name")] 
    public string FirstName { get; set; } = string.Empty;
    
    [Display("Last name")] 
    public string LastName { get; set; } = string.Empty;
    
    [Display("User email")] 
    public string? UserEmail { get; set; }
    
    [Display("Comment")] 
    public string Comment { get; set; } = string.Empty;
    
    [Display("User URL")] 
    public string UserUrl { get; set; } = string.Empty;
    
    [Display("State")] 
    public string State { get; set; } = string.Empty;
    
    [Display("User IP")] 
    public string? UserIp { get; set; }
    
    [Display("User referrer")] 
    public string? UserReferrer { get; set; }
    
    [Display("User agent")] 
    public string? UserAgent { get; set; }
    
    [Display("Content author email")] 
    public string? ContentAuthorEmail { get; set; }
    
    [Display("Content author name")] 
    public string? ContentAuthorName { get; set; }
    
    [Display("Content created at")] 
    public long ContentCreatedAt { get; set; }
    
    [Display("Thread ID")] 
    public string ThreadId { get; set; } = string.Empty;
    
    [Display("Replying to")] 
    public string? ReplyingTo { get; set; }
    
    [Display("Parent ID")] 
    public long ParentId { get; set; }
    
    [Display("Legacy ID")] 
    public long LegacyId { get; set; }
    
    [Display("Extra context")] 
    public string? ExtraContext { get; set; }
    
    [Display("Parent")] 
    public BlogCommentDto? Parent { get; set; }
}
