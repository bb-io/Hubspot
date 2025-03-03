using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses;

public class TranslatedLocalesResponse
{
    [Display("Primary language")]
    public string PrimaryLanguage { get; set; } = string.Empty;
    
    [Display("Translation language codes")]
    public List<string> TranslationLanguageCodes { get; set; } = new();
}