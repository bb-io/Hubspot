namespace Apps.Hubspot.Models.Responses;

public class GetAllResponse<TEntity>
{
    public long Total { get; set; }
    
    public List<TEntity> Results { get; set; } = new();
    
    public PagingInfo? Paging { get; set; }
}