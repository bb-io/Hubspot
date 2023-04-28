using Apps.Hubspot.Webhooks.Handlers.Models;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using RestSharp;

namespace Apps.Hubspot.Webhooks.Handlers
{
    public class ContactEmailChangesHandler : IWebhookEventHandler
    {

        const string SubscriptionEvent = "contact.propertyChange";
        const string PropertyName = "email";

        public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
            var appId = values["appId"];
            var subscribeRequuest = new WebhookSubscribeRequest
            {
                Active = true,
                EventType = SubscriptionEvent,
                PropertyName = PropertyName
            };
            string url = $"https://api.hubapi.com";
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/webhooks/v3/{appId}/subscriptions", Method.Post, authenticationCredentialsProviders);     
            request.AddHeader("content-type", "application/json");
            request.AddJsonBody(subscribeRequuest);
            await client.ExecuteAsync(request);    
        }

        public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
            var appId = values["appId"];
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/webhooks/v3/{appId}/subscriptions", Method.Get, authenticationCredentialsProviders);
            request.AddHeader("content-type", "application/json");
            var webhooks = client.Get<WebhookSubscribeResponse>(request);
            
            var subscription = webhooks.Results.FirstOrDefault(s => s.PropertyName == PropertyName && s.EventType == SubscriptionEvent);
            if (subscription == null) 
            {
                return;
            }
            var requestDelete = new HubspotRequest($"/webhooks/v3/{appId}/subscriptions/{subscription.Id}", Method.Delete, authenticationCredentialsProviders);
            await client.ExecuteAsync(requestDelete);
        }

        private AuthenticationCredentialsProvider GetAuthenticationCredentialsProvider(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            return authenticationCredentialsProviders.First(p => p.KeyName == "hapikey");
        }
    }
}
