using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos.Pages;

public class GenericPageDto : PageDto
{
    public Dictionary<string, JObject> Translations { get; set; }

    public JObject LayoutSections { get; set; }
}