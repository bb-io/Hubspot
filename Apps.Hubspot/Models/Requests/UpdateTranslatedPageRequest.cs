using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Requests;

public class UpdateTranslatedPageRequest
{
    public string ObjectId { get; set; }
    public string HtmlTitle { get; set; }
    public JObject LayoutSections { get; set; }
}