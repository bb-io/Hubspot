using Apps.Hubspot.Models.Dtos.Blogs.Posts;

namespace Apps.Hubspot.Webhooks.Models;

public class PageMemory
{
    public List<PageEntity> Pages { get; set; } = new();

    public PageMemory()
    { }
    
    public PageMemory(List<BlogPostDto> blogPosts)
    {
        Pages = blogPosts.Select(p => new PageEntity(p.Id, p.Created, p.Updated)).ToList();
    }
}

public class PageEntity : IEntity
{
    public string Id { get; set; } = string.Empty;
    
    public string Created { get; set; } = string.Empty;
    
    public string Updated { get; set; } = string.Empty;

    public PageEntity()
    { }
    
    public PageEntity(string id, string created, string updated)
    {
        Id = id;
        Created = created;
        Updated = updated;
    }
}