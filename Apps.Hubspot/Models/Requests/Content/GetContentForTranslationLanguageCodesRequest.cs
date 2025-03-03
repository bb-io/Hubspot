using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.Content;

public class GetContentForTranslationLanguageCodesRequest : ContentTypeFilter
{
    [Display("Content type"), StaticDataSource(typeof(ContentTypeForTranslationLanguageCodesHandler))]
    public override string ContentType { get; set; } = default!;
    
    [Display("Content ID"), DataSource(typeof(ContentDataHandler))]
    public string ContentId { get; set; } = string.Empty;
}