using Apps.Hubspot.Models.Dtos.Emails;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Webhooks.Models;

public class MarketingEmailPollingDto : MarketingEmailDto
{
    [Display("Event type")]
    public string EventType { get; set; } = string.Empty;

    public MarketingEmailPollingDto(MarketingEmailDto original, string eventType)
    { 
        Id = original.Id;
        Name = original.Name;
        Subject = original.Subject;
        PublishedAt = original.PublishedAt;
        UpdatedAt = original.UpdatedAt;
        CreatedAt = original.CreatedAt;
        Language = original.Language;
        State = original.State;
        Type = original.Type;
        IsPublished = original.IsPublished;
        Archived = original.Archived;
        ActiveDomain = original.ActiveDomain;
        EventType = eventType;
    }
}
