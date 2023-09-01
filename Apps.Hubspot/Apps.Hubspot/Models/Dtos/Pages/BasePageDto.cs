using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos.Pages;

public class BasePageDto
{
    [Display("Page ID")]
    public string Id { get; set; }
    
    [Display("Currently published")]
    public bool CurrentlyPublished { get; set; }
    
    [Display("Current state")]
    public string CurrentState { get; set; }
    
    [Display("HTML title")]
    public string HtmlTitle { get; set; }
    
    [Display("Layour sections")]
    public JObject LayoutSections { get; set; }
}