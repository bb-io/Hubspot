using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Responses;
public class ActivityInfo
{
    [JsonProperty("portalId")]
    public long PortalId { get; set; }

    [JsonProperty("timeZone")]
    public string TimeZone { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("utcOffset")]
    public string UtcOffset { get; set; }
}
