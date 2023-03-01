namespace Apps.Hubspot.Models.Deals
{
    public class BaseDealProperties
    {
        public decimal Amount { get; set; }   
        public string? Dealname { get; set; }
        public string? Dealstage { get; set; }        
        public string? Hs_owner_id { get; set; }
        public string? Pipeline { get; set; }
    }
}
