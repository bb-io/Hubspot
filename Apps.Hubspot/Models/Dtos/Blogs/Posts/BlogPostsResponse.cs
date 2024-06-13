using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Posts;

public class BlogPostsResponse
{
    [Display("Blog posts")]
    public List<BlogPostDto> BlogPosts { get; set; } = new();
}