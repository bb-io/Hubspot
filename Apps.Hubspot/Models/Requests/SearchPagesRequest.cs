using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class SearchPagesRequest : TimeFilterRequest
{
    [Display("Not translated in language"), StaticDataSource(typeof(LanguageHandler))]
    [JsonIgnore]
    public string? NotTranslatedInLanguage { get; set; }

    [Display("Language",Description = "Does not support language locale values (e.g., en-us)"), StaticDataSource(typeof(LanguageHandler))]
    [JsonProperty("language__in")]
    public string? Language { get; set; }

    [Display("Site name")] // TODO: datahandler
    [JsonProperty("name__icontains")]
    public string? Name { get; set; }

    [Display("Slug")] //TODO: datahandler
    [JsonProperty("slug__eq")]
    public string? Slug { get; set; }

    [Display("State")] //TODO: datahandler
    [JsonProperty("state__eq")]
    public string? State { get; set; }
}