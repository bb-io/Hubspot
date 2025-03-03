using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Responses.Translations;

public class TranslationResponse
{
    [Display("Translation ID")]
    public string TranslationId { get; set; } = string.Empty;

    [Display("Page ID")]
    public string PageId { get; set; } = string.Empty;
}