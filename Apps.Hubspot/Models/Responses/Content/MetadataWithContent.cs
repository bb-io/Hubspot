using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Responses.Content;
public class MetadataWithContent : Metadata
{
    public FileReference Content { get; set; }
}
