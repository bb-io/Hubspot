using Apps.Common.Http;
using Apps.Hubspot.Dtos.Contacts;
using Apps.Hubspot.Models.Contacts;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class ContactActions : BaseActions<Contact, CreateOrUpdateContact>
    {
        public ContactActions() : base(new HttpRequestProvider())
        {
        }

        [Action]
        public IEnumerable<ContactDto> GetContacts(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetAll(requestUrl, null, authenticationCredentialsProvider).Select(CreateDtoByEntity).ToList();
        }

        [Action]
        public ContactDto? GetContact(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] int contactId
            )
        {
            var requestUrl = GetRequestUrl(url);
            var contact = GetOne(requestUrl, contactId, null, authenticationCredentialsProvider);
            return contact != null
                ? CreateDtoByEntity(contact)
                : throw new InvalidOperationException($"Cannot get company: {contactId}");
        }

        [Action]
        public ContactDto? CreateContact(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] CreateOrUpdateContactDto dto
            )
        {
            var requestUrl = GetRequestUrl(url);
            var contact = CreateDtoByEntity(dto);
            var createdContact = Create(requestUrl, null, contact, authenticationCredentialsProvider);
            return createdContact != null
                ? CreateDtoByEntity(createdContact)
                : null;
        }

        [Action]
        public ContactDto? UpdateContact(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] int contactId,
            [ActionParameter] CreateOrUpdateContactDto dto
            )
        {
            var requestUrl = GetRequestUrl(url);
            var contact = CreateDtoByEntity(dto);
            var updatedContact = Update(requestUrl, contactId, null, contact, authenticationCredentialsProvider);
            return updatedContact != null
                ? CreateDtoByEntity(updatedContact)
                : throw new InvalidOperationException($"Cannot update company: {contactId}");
        }

        [Action]
        public void DeleteContact(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] int contactId
            )
        {
            var requestUrl = GetRequestUrl(url);
            Delete(requestUrl, contactId, null, authenticationCredentialsProvider);
        }

        private string GetRequestUrl(string url)
        {
            const string requestUrlFormat = "{0}/crm/v3/objects/contacts";
            return string.Format(requestUrlFormat, url);
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
