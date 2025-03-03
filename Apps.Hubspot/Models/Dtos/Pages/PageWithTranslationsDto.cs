using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Pages;

public class PageWithTranslationsDto : PageDto
{
    [DefinitionIgnore]
    public Dictionary<string, ObjectWithId> Translations { get; set; } = new();
}