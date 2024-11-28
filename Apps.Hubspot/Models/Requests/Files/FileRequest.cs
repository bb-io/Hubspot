using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.Files;

public class FileRequest
{
    [Display("File")]
    public FileReference File { get; set; }
}