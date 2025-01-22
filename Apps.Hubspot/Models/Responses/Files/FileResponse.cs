using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Responses.Files;

public class FileResponse
{
    public FileReference File { get; set; } = default!;
}