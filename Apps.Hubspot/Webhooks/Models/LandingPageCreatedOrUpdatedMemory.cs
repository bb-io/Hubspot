using Apps.Hubspot.Models.Dtos.Pages;

namespace Apps.Hubspot.Webhooks.Models;

public class LandingPageCreatedOrUpdatedMemory
{
    public List<PageDto> Pages { get; set; }
}