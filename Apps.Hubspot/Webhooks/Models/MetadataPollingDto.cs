using Apps.Hubspot.Models.Responses.Content;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Webhooks.Models;

public class MetadataPollingDto : Metadata
{
    [Display("Event type")]
    public string EventType { get; set; } = string.Empty;
}
