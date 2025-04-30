using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Requests;

public class UpdateTranslatedPageRequest
{
    public string Id { get; set; }
    public string HtmlTitle { get; set; }
    public JObject LayoutSections { get; set; }
    public string Slug { get; set; }
    public string metaDescription { get; set; }
}