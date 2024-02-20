using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests.Emails;

public class SearchEmailsRequest : TimeFilterRequest
{
    [Display("Published")]
    [JsonProperty("isPublished")]
    public bool? IsPublished { get; set; }

    [Display("Archived")]
    [JsonProperty("archived")]
    public bool? Archived { get; set; }
}