using Apps.Hubspot.Webhooks.Handlers;
using Apps.Hubspot.Webhooks.Payloads;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace Apps.Hubspot.Webhooks
{
    [WebhookList]
    public class WebhookList
    {
        [Webhook("On company created", typeof(CompanyCreationHandler), Description = "On company created")]
        public async Task<WebhookResponse<CompanyCreatedPayload>> ProjectCreatedHandler(WebhookRequest webhookRequest)
        {
            var data = JsonConvert.DeserializeObject<CompanyCreatedPayload>(webhookRequest.Body.ToString());
            if (data is null) { throw new InvalidCastException(nameof(webhookRequest.Body)); }
            return new WebhookResponse<CompanyCreatedPayload>
            {
                HttpResponseMessage = null,
                Result = data
            };
        }
    }
}
