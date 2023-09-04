using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos.Pages;

public class GenericPageDto : PageDto
{
    public JObject Translations { get; set; }

    public JObject LayoutSections { get; set; }
}