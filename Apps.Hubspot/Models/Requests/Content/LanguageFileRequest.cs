using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Hubspot.Models.Requests.Content;

public class LanguageFileRequest : IUploadContentInput
{
    public FileReference Content { get; set; } = default!;
    
    [Display("Target language"), StaticDataSource(typeof(LanguageHandler))]
    public string Locale { get; set; } = default!;

    [Display("Content ID"), DataSource(typeof(ContentDataHandler))]
    public string? ContentId { get; set; }
}