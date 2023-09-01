using Blackbird.Applications.Sdk.Common;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Hubspot.Models.Responses.Files;

public class FileResponse
{
    [Display("File")]
    public File File { get; set; }

    [Display("File language")]
    public string FileLanguage { get; set; }
}