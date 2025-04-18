using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
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

    [Display("Site name")]
    [JsonProperty("name__icontains")]
    public string? Name { get; set; }

    [Display("Slug")]
    [JsonProperty("slug__eq")]
    public string? Slug { get; set; }

    [Display("State")]
    [JsonProperty("state__eq")]
    public string? State { get; set; }
}