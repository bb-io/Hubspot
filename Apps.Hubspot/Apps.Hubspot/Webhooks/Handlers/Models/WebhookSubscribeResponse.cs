namespace Apps.Hubspot.Webhooks.Handlers.Models
{
    internal class WebhookSubscribeResponse
    {
        public IEnumerable<WebhookSubscription> Results { get; set; }
    }
}
