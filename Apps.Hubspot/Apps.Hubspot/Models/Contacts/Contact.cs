namespace Apps.Hubspot.Models.Contacts
{
    public class Contact
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Archived { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ContactProperties Properties { get;set;}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class ContactProperties : BaseContactProperties
    {
        public DateTime? Createdate { get; set; }
        public int? Hs_object_id { get; set; }
        public DateTime? Lastmodifieddate { get; set; }
    }
}
