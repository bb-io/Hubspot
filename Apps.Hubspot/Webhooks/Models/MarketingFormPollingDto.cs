using Apps.Hubspot.Models.Dtos.Forms;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Webhooks.Models;

public record MarketingFormPollingDto : MarketingFormDto
{
    public MarketingFormPollingDto()
    { }

    public MarketingFormPollingDto(MarketingFormDto original, string eventType)
    {
        Id = original.Id;
        Name = original.Name;
        CreatedAt = original.CreatedAt;
        UpdatedAt = original.UpdatedAt;
        Archived = original.Archived;
        FormType = original.FormType;
        Configuration = original.Configuration;
        FieldGroups = original.FieldGroups;
        EventType = eventType;
    }

    [Display("Event type")]
    public string EventType { get; set; } = string.Empty;
}
