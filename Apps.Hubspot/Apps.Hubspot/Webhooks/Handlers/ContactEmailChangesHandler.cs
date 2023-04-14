using Apps.Hubspot.Http;
using Apps.Hubspot.Webhooks.Handlers.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Hubspot.Webhooks.Handlers
{
    public class ContactEmailChangesHandler : RestRequestProvider, IWebhookEventHandler
    {
        protected Dictionary<string, string> RequestWithBodyHeaders = new Dictionary<string, string>
            {
                { "content-type", "application/json" }
            };

        const string SubscriptionEvent = "contact.propertyChange";
        const string PropertyName = "email";

        public ContactEmailChangesHandler() : base(new HttpRequestProvider())
        {
        }

        public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
            var appId = values["appId"];
            var subscribeRequuest = new WebhookSubscribeRequest
            {
                Active = true,
                EventType = SubscriptionEvent,
                PropertyName = PropertyName
            };
            string url = $"https://api.hubapi.com/webhooks/v3/{appId}/subscriptions";
            var authenticationCredentialsProvider = GetAuthenticationCredentialsProvider(authenticationCredentialsProviders);
            await CreateAsync<WebhookSubscribeRequest, object>(url, null, RequestWithBodyHeaders, subscribeRequuest, authenticationCredentialsProvider);           
        }

        public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
            var appId = values["appId"];
            var url = $"https://api.hubapi.com/webhooks/v3/{appId}/subscriptions";
            var authenticationCredentialsProvider = GetAuthenticationCredentialsProvider(authenticationCredentialsProviders);
            var subsriptions = await GetAllAsync<WebhookSubscribeResponse>(url, null, RequestWithBodyHeaders, authenticationCredentialsProvider);
            if(subsriptions == null)
            {
                return;
            }
            var subscription = subsriptions.Results.FirstOrDefault(s => s.PropertyName == PropertyName && s.EventType == SubscriptionEvent);
            if (subscription == null) 
            {
                return;
            }
            url += $"/{subscription.Id}";            
            await DeleteAsync<object>(url, null, RequestWithBodyHeaders, authenticationCredentialsProvider);
        }

        private AuthenticationCredentialsProvider GetAuthenticationCredentialsProvider(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            return authenticationCredentialsProviders.First(p => p.KeyName == "hapikey");
        }
    }
}
