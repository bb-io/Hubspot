using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.Content;

public class LanguageFileRequest
{
    public FileReference File { get; set; } = default!;
    
    [Display("Target language"), StaticDataSource(typeof(LanguageHandler))]
    public string TargetLanguage { get; set; } = default!;
}