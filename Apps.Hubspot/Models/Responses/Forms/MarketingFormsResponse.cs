using Apps.Hubspot.Models.Dtos.Forms;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Forms;

public class MarketingFormsResponse
{
    [Display("Marketing forms")]
    public List<MarketingFormDto> Forms { get; set; } = new();
    
    public MarketingFormsResponse()
    { }
    
    public MarketingFormsResponse(List<MarketingFormDto> pages)
    {
        Forms = pages;
    }
}