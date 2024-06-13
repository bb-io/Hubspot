using Apps.Hubspot.Models.Dtos.Blogs.Posts;

namespace Apps.Hubspot.Webhooks.Models;

public class BlogPostsCreatedOrUpdatedMemory
{
    public List<BlogPostDto> BlogPosts { get; set; } = new();
}