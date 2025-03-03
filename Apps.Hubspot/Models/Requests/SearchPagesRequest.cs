using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class SearchPagesRequest : TimeFilterRequest
{
    [Display("Not translated in language"), StaticDataSource(typeof(LanguageHandler))]
    public string? NotTranslatedInLanguage { get; set; }

    [Display("Language"), StaticDataSource(typeof(LanguageHandler))]
    public string? Language { get; set; }

    [Display("Domain"), DataSource(typeof(DomainDataHandler)), JsonIgnore]
    public string? Domain { get; set; }
    
    [Display("Current state"), StaticDataSource(typeof(CurrentStateHandler)), JsonIgnore]
    public string? CurrentState { get; set; }
}