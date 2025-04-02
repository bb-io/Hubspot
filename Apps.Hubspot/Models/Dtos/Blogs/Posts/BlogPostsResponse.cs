using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Posts;

public class BlogPostsResponse
{
    [Display("Blog posts")]
    public List<BlogPostPollingDto> BlogPosts { get; set; } = new();

    public BlogPostsResponse()
    { }
    
    public BlogPostsResponse(List<BlogPostPollingDto> blogPosts)
    {
        BlogPosts = blogPosts;
    }
}