using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Emails;

public class MarketingEmailsResponse
{
    [Display("Marketing emails")]
    public List<MarketingEmailPollingDto> Emails { get; set; } = new();
    
    public MarketingEmailsResponse()
    { }
    
    public MarketingEmailsResponse(List<MarketingEmailPollingDto> pages)
    {
        Emails = pages;
    }
}