namespace Apps.Hubspot.Models.Deals
{
    public class CreateOrUpdateDeal
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CreateOrUpdateDealProperties Properties { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class CreateOrUpdateDealProperties : BaseDealProperties
    {
        public int Hubspot_owner_id { get; set; }
        public string? Closedate { get; set; }
    }
}
