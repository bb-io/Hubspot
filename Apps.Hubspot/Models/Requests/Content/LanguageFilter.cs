using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests.Content;
public class LanguageFilter
{
    [Display("Language ID"), StaticDataSource(typeof(LanguageHandler)), JsonProperty("language__in")]
    public string? Language { get; set; } 
}
