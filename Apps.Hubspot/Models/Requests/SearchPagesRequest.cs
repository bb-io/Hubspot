using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.DataSourceHandlers.Static;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class SearchPagesRequest : PagesTimeFilterRequest
{
    [Display("Not translated in language"), StaticDataSource(typeof(LanguageHandler))]
    [JsonIgnore]
    public string? NotTranslatedInLanguage { get; set; }

    [Display("Language", Description = "Does not support language locale values (e.g., en-us)"), StaticDataSource(typeof(LanguageHandler))]
    [JsonProperty("language__in")]
    public string? Language { get; set; }

    [Display("Site name contains")]
    [JsonProperty("name__icontains")]
    public string? NameContains { get; set; }

    [Display("Site name equals")]
    [JsonProperty("name__eq")]
    public string? NameEquals { get; set; }

    [Display("Slug")]
    [JsonProperty("slug__eq")]
    public string? Slug { get; set; }

    [Display("State")]
    [JsonProperty("state__in"), StaticDataSource(typeof(StateHandler))]
    public List<string>? State { get; set; }

    [Display("Domain equals"), DataSource(typeof(DomainDataHandler))]
    [JsonProperty("domain__eq")]
    public string? PageDomain { get; set; }

    [Display("Domain contains"), DataSource(typeof(DomainDataHandler))]
    [JsonProperty("domain__contains")]
    public string? PageDomainContains { get; set; }

    [Display("A/B status"), StaticDataSource(typeof(ABStatusDataHandler))]
    [JsonProperty("abStatus__eq")]
    public string? AbStatus { get; set; }

    [Display("Url contains")]
    [JsonIgnore]
    public string? UrlContains { get; set; }

    [Display("Updated by user IDs (whitelist)")]
    [JsonIgnore]
    public IEnumerable<string>? UpdatedByUserIdsWhitelist { get; set; }

    [Display("Updated by user IDs (blacklist)")]
    [JsonIgnore]
    public IEnumerable<string>? UpdatedByUserIdsBlacklist { get; set; }
}