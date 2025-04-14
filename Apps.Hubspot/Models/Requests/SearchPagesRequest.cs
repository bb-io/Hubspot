using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class SearchPagesRequest : TimeFilterRequest
{
    [Display("Not translated in language"), StaticDataSource(typeof(LanguageHandler))]
    public string? NotTranslatedInLanguage { get; set; }

    [Display("Language"), StaticDataSource(typeof(LanguageHandler))]
    [JsonProperty("language__in")]
    public string? Language { get; set; }

    [Display("Site name")] // TODO: datahandler
    [JsonProperty("__icontains")]
    public string? Name { get; set; }

    [Display("Slug")] // TODO: datahandler
    [JsonProperty("slug__icontains")]
    public string? Slug { get; set; }
}