namespace Apps.Hubspot.Models.Companies
{
    public class CreateOrUpdateCompany
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CreateOrUpdateCompanyProperties Properties { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class CreateOrUpdateCompanyProperties : BaseCompanyProperties
    {
        public string? City { get; set; }
        public string? Industry { get; set; }
        public string? Phone { get; set; }
        public string? State { get; set; }
    }
}
