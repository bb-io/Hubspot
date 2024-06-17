using Apps.Hubspot.Models.Dtos.Pages;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Pages;

public class PagesResponse
{
    [Display("Pages")]
    public List<PageDto> Pages { get; set; } = new();
    
    public PagesResponse()
    { }
    
    public PagesResponse(List<PageDto> pages)
    {
        Pages = pages;
    }
}