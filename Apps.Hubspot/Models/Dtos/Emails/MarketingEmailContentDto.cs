using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos.Emails;

public class MarketingEmailContentDto
{
    public JObject Content { get; set; }
}