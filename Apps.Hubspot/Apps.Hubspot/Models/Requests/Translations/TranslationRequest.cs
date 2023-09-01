using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.Translations;

public class TranslationRequest
{
    public string Language { get; set; }
    
    [Display("Primary language")]
    public string PrimaryLanguage { get; set; }
}