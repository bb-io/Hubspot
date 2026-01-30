using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses;

public class TranslatedLocalesResponse
{
    [Display("Primary language")]
    public string PrimaryLanguage { get; set; } = string.Empty;
    
    [Display("Translation language codes")]
    public List<string> TranslationLanguageCodes { get; set; } = new();

    [Display("Translations")]
    public List<Translation> Translations { get; set; } = [];
}

public class Translation
{
    [Display("Content ID")]
    public string Id { get; set; } = string.Empty;

    [Display("Slug")]
    public string Slug { get; set; } = string.Empty;

    [Display("Language code")]
    public string LanguageCode { get; set; } = string.Empty;
}