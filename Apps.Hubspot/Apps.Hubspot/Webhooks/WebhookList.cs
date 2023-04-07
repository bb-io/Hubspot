using Apps.Hubspot.Webhooks.Handlers;
using Apps.Hubspot.Webhooks.Payloads;
using Blackbird.Applications.Sdk.Common.Webhooks;
using System.Text.Json;

namespace Apps.Hubspot.Webhooks
{
    [WebhookList]
    public class WebhookList
    {
        [Webhook(typeof(ContactEmailChangesHandler))]
        public ContactEmailChangedPayload? ContactEmailChanged(object payload)
        {
            var payloadString = payload.ToString();
            if(string.IsNullOrEmpty(payloadString))
            {
                throw new InvalidCastException(nameof(payload));
            }
            return JsonSerializer.Deserialize<IEnumerable<ContactEmailChangedPayload>>(payloadString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true})?.FirstOrDefault();
        }
    }
}
