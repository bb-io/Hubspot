using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Forms;

public class MarketingFormsResponse
{
    [Display("Marketing forms")]
    public List<MarketingFormPollingDto> Forms { get; set; } = new();
    
    public MarketingFormsResponse()
    { }
    
    public MarketingFormsResponse(List<MarketingFormPollingDto> pages)
    {
        Forms = pages;
    }
}