namespace Apps.Hubspot.Webhooks.Models;

public class PageMemory
{
    public List<PageEntity> Pages { get; set; } = new();
}

public class PageEntity : IEntity
{
    public string Id { get; set; }
    
    public string Created { get; set; }
    
    public string Updated { get; set; }
}