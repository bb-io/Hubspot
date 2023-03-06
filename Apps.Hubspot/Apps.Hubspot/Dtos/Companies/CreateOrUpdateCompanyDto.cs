namespace Apps.Hubspot.Dtos.Companies
{
    public class CreateOrUpdateCompanyDto : BaseCompanyDto
    {
        public string? City { get; set; }
        public string? Industry { get; set; }
        public string? Phone { get; set; }
        public string? State { get; set; }
    }
}
