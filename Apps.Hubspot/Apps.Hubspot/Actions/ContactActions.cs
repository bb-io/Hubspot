using Apps.Hubspot.Dtos.Contacts;
using Apps.Hubspot.Models.Contacts;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Actions;
using Apps.Hubspot.Models.Companies;
using Apps.Hubspot.Models;
using RestSharp;
using System.ComponentModel.Design;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class ContactActions
    {
        [Action("Get all contacts", Description = "Get a list of all contacts")]
        public IEnumerable<ContactDto> GetContacts(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest("/crm/v3/objects/contacts", Method.Get, authenticationCredentialsProviders);
            return client.Get<GetAllResponse<Contact>>(request).Results.Select(c => CreateDtoByEntity(c)).ToList();
        }

        [Action("Get contact", Description = "Get information of specific contact")]
        public ContactDto? GetContact(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] int contactId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/contacts/{contactId}", Method.Get, authenticationCredentialsProviders);
            return CreateDtoByEntity(client.Get<Contact>(request));
        }

        [Action("Create contact", Description = "Create a new contact")]
        public ContactDto? CreateContact(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateOrUpdateContactDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/contacts", Method.Post, authenticationCredentialsProviders);
            var contact = CreateDtoByEntity(dto);
            request.AddJsonBody(contact);
            return CreateDtoByEntity(client.Post<Contact>(request));
        }

        [Action("Update contact", Description = "Update a contact's information")]
        public ContactDto? UpdateContact(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] int contactId, [ActionParameter] CreateOrUpdateContactDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/contacts", Method.Patch, authenticationCredentialsProviders);
            var contact = CreateDtoByEntity(dto);
            request.AddJsonBody(contact);
            return CreateDtoByEntity(client.Patch<Contact>(request));
        }

        [Action("Delete contact", Description = "Delete a contact")]
        public void DeleteContact(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] int contactId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/contacts/{contactId}", Method.Delete, authenticationCredentialsProviders);
            client.Execute(request);
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
