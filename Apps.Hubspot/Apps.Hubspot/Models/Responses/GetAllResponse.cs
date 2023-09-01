namespace Apps.Hubspot.Models.Responses;

public class GetAllResponse<TEntity>
{
    public IEnumerable<TEntity> Results { get; set; }
}