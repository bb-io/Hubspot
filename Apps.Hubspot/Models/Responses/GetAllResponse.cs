namespace Apps.Hubspot.Models.Responses;

public class GetAllResponse<TEntity>
{
    public long Total { get; set; }
    public IEnumerable<TEntity> Results { get; set; }
    
    public PagingInfo? Paging { get; set; }
}