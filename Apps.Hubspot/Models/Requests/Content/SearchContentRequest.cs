using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests.Content;

public class SearchContentRequest
{
    [Display("Domain equals"), DataSource(typeof(DomainDataHandler))]
    [JsonProperty("domain__eq")]
    public string? Domain { get; set; }

    [Display("Domain contains")]
    [JsonProperty("domain__contains")]
    public string? DomainContains { get; set; }

    [Display("Current state"), StaticDataSource(typeof(CurrentStateHandler))]
    [JsonProperty("state__eq")]
    public string? CurrentState { get; set; }

    [Display("URL contains"), JsonIgnore]
    public string? UrlContains { get; set; }

    [Display("Slug"), JsonProperty("slug")]
    public string? Slug { get; set; }

    [Display("Title contains")]
    [JsonProperty("name__icontains")]
    public string? NameContains { get; set; }

    [Display("Title equals")]
    [JsonProperty("name__eq")]
    public string? NameEquals { get; set; }

    [Display("Updated by user IDs (whitelist)")]
    [JsonIgnore]
    public IEnumerable<string>? UpdatedByUserIdsWhitelist { get; set; }

    [Display("Updated by user IDs (blacklist)")]
    [JsonIgnore]
    public IEnumerable<string>? UpdatedByUserIdsBlacklist { get; set; }
}