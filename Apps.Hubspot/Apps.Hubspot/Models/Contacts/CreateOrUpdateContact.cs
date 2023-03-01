namespace Apps.Hubspot.Models.Contacts
{
    public class CreateOrUpdateContact
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CreateOrUpdateContactProperties Properties { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public class CreateOrUpdateContactProperties : BaseContactProperties
    {
    }
}
