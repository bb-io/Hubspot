using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Pages;

public class PagesResponse
{
    [Display("Pages")]
    public List<PagePollingDto> Pages { get; set; } = new();
    
    public PagesResponse()
    { }
    
    public PagesResponse(List<PagePollingDto> pages)
    {
        Pages = pages;
    }
}