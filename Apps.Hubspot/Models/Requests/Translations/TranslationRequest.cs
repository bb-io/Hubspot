using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.Translations;

public class TranslationRequest
{
    [StaticDataSource(typeof(LanguageHandler))]
    public string Language { get; set; }
    
    [Display("Primary language")]
    [StaticDataSource(typeof(LanguageHandler))]
    public string PrimaryLanguage { get; set; }
}