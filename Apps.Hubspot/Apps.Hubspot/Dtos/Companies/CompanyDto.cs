namespace Apps.Hubspot.Dtos.Companies
{
    public class CompanyDto : BaseCompanyDto
    {
        public string Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Archived { get; set; }
        public DateTime? Createdate { get; set; }
        public DateTime? Hs_lastmodifieddate { get; set; }
        public string? Hs_object_id { get; set; }
    }
}
