namespace Apps.Hubspot.Models.Deals
{
    public class Deal
    {
        public long Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Archived { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DealProperties Properties { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class DealProperties : BaseDealProperties
    {
        public DateTime Closedate { get; set; }
        public DateTime? Createdate { get; set; }
        public DateTime? Hs_lastmodifieddate { get; set; }
        public long Hs_object_id { get; set; }
    }
}
