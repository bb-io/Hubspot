using Apps.Hubspot.Http;
using Apps.Hubspot.Dtos.Contacts;
using Apps.Hubspot.Models.Contacts;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Actions;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class ContactActions : BaseActions<Contact, CreateOrUpdateContact>
    {
        private readonly string _requestUrl = "https://api.hubapi.com/crm/v3/objects/contacts";
        public ContactActions() : base(new HttpRequestProvider())
        {
        }

        [Action]
        public IEnumerable<ContactDto> GetContacts(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders
            )
        {
            return GetAll(_requestUrl, null, authenticationCredentialsProviders).Select(CreateDtoByEntity).ToList();
        }

        [Action]
        public ContactDto? GetContact(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] int contactId
            )
        {
            var contact = GetOne(_requestUrl, contactId, null, authenticationCredentialsProviders);
            return contact != null
                ? CreateDtoByEntity(contact)
                : throw new InvalidOperationException($"Cannot get company: {contactId}");
        }

        [Action]
        public ContactDto? CreateContact(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateOrUpdateContactDto dto
            )
        {
            var contact = CreateDtoByEntity(dto);
            var createdContact = Create(_requestUrl, null, contact, authenticationCredentialsProviders);
            return createdContact != null
                ? CreateDtoByEntity(createdContact)
                : null;
        }

        [Action]
        public ContactDto? UpdateContact(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] int contactId,
            [ActionParameter] CreateOrUpdateContactDto dto
            )
        {
            var contact = CreateDtoByEntity(dto);
            var updatedContact = Update(_requestUrl, contactId, null, contact, authenticationCredentialsProviders);
            return updatedContact != null
                ? CreateDtoByEntity(updatedContact)
                : throw new InvalidOperationException($"Cannot update company: {contactId}");
        }

        [Action]
        public void DeleteContact(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] int contactId
            )
        {
            Delete(_requestUrl, contactId, null, authenticationCredentialsProviders);
        }

        private ContactDto CreateDtoByEntity(Contact contact)
        {
            return new ContactDto
            {
                Id = contact.Id,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt,
                Archived = contact.Archived,
                Email = contact.Properties.Email,
                Firstname = contact.Properties.Firstname,
                Lastname = contact.Properties.Lastname,
                Createdate = contact.Properties.Createdate,
                Hs_lastmodifieddate = contact.Properties.Lastmodifieddate,
                Hs_object_id = contact.Properties.Hs_object_id
            };
        }

        private CreateOrUpdateContact CreateDtoByEntity(CreateOrUpdateContactDto dto)
        {
            return new CreateOrUpdateContact
            {
                Properties = new CreateOrUpdateContactProperties
                {
                    Email = dto.Email,
                    Firstname = dto.Firstname,
                    Lastname = dto.Lastname
                }
            };
        }
    }
}
