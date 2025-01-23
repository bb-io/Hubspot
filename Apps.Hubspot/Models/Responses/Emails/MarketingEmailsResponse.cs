using Apps.Hubspot.Models.Dtos.Emails;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Emails;

public class MarketingEmailsResponse
{
    [Display("Marketing emails")]
    public List<MarketingEmailDto> Emails { get; set; } = new();
    
    public MarketingEmailsResponse()
    { }
    
    public MarketingEmailsResponse(List<MarketingEmailDto> pages)
    {
        Emails = pages;
    }
}