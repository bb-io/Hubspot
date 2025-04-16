using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests.Emails;

public class SearchEmailsRequest : TimeFilterRequest
{
    [Display("Archived")]
    [JsonProperty("archived")]
    public bool? Archived { get; set; }

    [StaticDataSource(typeof(LanguageHandler))]
    [Display("Language")]
    [JsonProperty("language")]
    public string? Language { get; set; }

    [Display("Created at")]
    [JsonProperty("createdAt__eq")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? CreatedAt { get; set; }//TODO: remove, WIP

    [Display("Created after")]
    [JsonProperty("createdAt__gt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? CreatedAfter { get; set; }//TODO: remove, WIP

    [Display("Created before")]
    [JsonProperty("createdAt__lt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? CreatedBefore { get; set; } //TODO: remove, WIP
}