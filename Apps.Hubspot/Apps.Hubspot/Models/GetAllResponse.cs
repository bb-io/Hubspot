namespace Apps.Hubspot.Models
{
    public class GetAllResponse<TEntity>
    {
        public IEnumerable<TEntity> Results { get; set; }
    }
}
