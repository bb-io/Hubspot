namespace Apps.Hubspot.Dtos.Deals
{
    public class CreateOrUpdateDealDto : BaseDealDto
    {
        public int Hubspot_owner_id { get; set; }
        public string? Closedate { get; set; }
    }
}
