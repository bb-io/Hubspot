using Apps.Common.Http;
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
        public IEnumerable<Contact> GetContacts(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetAll(requestUrl, null, authenticationCredentialsProvider);
        }

        [Action]
        public Contact? GetContact(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] int contactId
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetOne(requestUrl, contactId, null, authenticationCredentialsProvider);
        }

        [Action]
        public Contact? CreateContact(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] CreateOrUpdateContact contact
            )
        {
            var requestUrl = GetRequestUrl(url);
            return Create(requestUrl, null, contact, authenticationCredentialsProvider);
        }

        [Action]
        public Contact? UpdateContact(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] int contactId,
            [ActionParameter] CreateOrUpdateContact contact
            )
        {
            var requestUrl = GetRequestUrl(url);
            return Update(requestUrl, contactId, null, contact, authenticationCredentialsProvider);
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
    }
}
