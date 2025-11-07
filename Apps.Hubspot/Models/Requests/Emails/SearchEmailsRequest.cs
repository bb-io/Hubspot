using Apps.Hubspot.DataSourceHandlers;
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
}