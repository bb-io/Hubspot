using Apps.Hubspot.Models.Dtos.Blogs.Comments;

namespace Apps.Hubspot.Models.Responses.Blogs.Comments;

public class GetBlogCommentsResponse
{
    public List<BlogCommentDto> Objects { get; set; } = new();
    
    public long Total { get; set; }
    
    public long Limit { get; set; }
    
    public long Offset { get; set; }
}
