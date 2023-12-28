using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Responses.Files;

public class FileResponse
{
    [Display("File")]
    public FileReference File { get; set; }

    [Display("File language")]
    public string FileLanguage { get; set; }
}