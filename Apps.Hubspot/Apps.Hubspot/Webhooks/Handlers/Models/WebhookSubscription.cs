namespace Apps.Hubspot.Webhooks.Handlers.Models
{
    internal class WebhookSubscription
    {
        public string Id { get; set; }
        public bool Active { get; set; }
        public string EventType { get; set; }
        public string PropertyName { get; set; }
    }
}
