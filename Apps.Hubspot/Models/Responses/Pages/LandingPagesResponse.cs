using Apps.Hubspot.Models.Dtos.Pages;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Pages;

public class LandingPagesResponse
{
    [Display("Landing pages")]
    public List<PageDto> Pages { get; set; } = new();
}