using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Responses.Files;

public class FileLanguageResponse
{
    [Display("File")]
    public FileReference File { get; set; } = default!;

    [Display("File language")]
    public string FileLanguage { get; set; } = default!;
}