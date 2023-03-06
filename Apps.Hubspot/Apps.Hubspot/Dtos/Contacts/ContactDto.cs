namespace Apps.Hubspot.Dtos.Contacts
{
    public class ContactDto : BaseContactDto
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Archived { get; set; }
        public DateTime? Createdate { get; set; }
        public DateTime? Hs_lastmodifieddate { get; set; }
        public long? Hs_object_id { get; set; }
    }
}
