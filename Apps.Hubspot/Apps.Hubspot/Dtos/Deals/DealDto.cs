namespace Apps.Hubspot.Dtos.Deals
{
    public class DealDto : BaseDealDto
    {
        public long Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Archived { get; set; }
        public DateTime Closedate { get; set; }
        public DateTime? Createdate { get; set; }
        public DateTime? Hs_lastmodifieddate { get; set; }
        public long Hs_object_id { get; set; }
    }
}
