using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Hubspot.Models.Responses.Files;

public class FileLanguageResponse : IDownloadContentOutput
{
    [Display("File")]
    public FileReference Content { get; set; } = default!;

    [Display("File language")]
    public string FileLanguage { get; set; } = default!;
}