using Apps.Hubspot.Webhooks.Handlers;
using Apps.Hubspot.Webhooks.Payloads;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Text.Json;

namespace Apps.Hubspot.Webhooks
{
    [WebhookList]
    public class WebhookList
    {
        [Webhook("On contact email changed", typeof(ContactEmailChangesHandler), Description = "Triggered when the email address of a contact has changed")]
        public ContactEmailChangedPayload? ContactEmailChanged(WebhookRequest webhookRequest)
        {
            var payloadString = webhookRequest.Body.ToString();
            if(string.IsNullOrEmpty(payloadString))
            {
                throw new InvalidCastException(nameof(webhookRequest.Body));
            }
            return JsonSerializer.Deserialize<IEnumerable<ContactEmailChangedPayload>>(payloadString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true})?.FirstOrDefault();
        }
    }
}
