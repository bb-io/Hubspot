using System.Globalization;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Dtos.Pages;

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
    
    public PageMemory(List<PageDto> blogPosts)
    {
        Pages = blogPosts.Select(p => new PageEntity(p.Id, p.Created.ToString(CultureInfo.InvariantCulture), p.Updated.ToString(CultureInfo.InvariantCulture))).ToList();
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
    
    public PageEntity(string id, DateTime created, DateTime updated)
    {
        Id = id;
        Created = created.ToString(CultureInfo.InvariantCulture);
        Updated = updated.ToString(CultureInfo.InvariantCulture);
    }
}