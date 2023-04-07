namespace Apps.Hubspot.Webhooks.Payloads
{
    public class ContactEmailChangedPayload
    {
        public long EventId { get; set; }
        public long SubscriptionId { get; set; }
        public long PortalId { get; set; }
        public long AppId { get; set; }
        public long OccurredAt { get; set; }
        public string SubscriptionType { get; set; }
        public long AttemptNumber { get; set; }
        public int ObjectId { get; set; }
	    public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public string ChangeSource { get; set; }
        public string SourceId { get; set; }
    }
}
