using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.Content;

public class ContentTypeFilter
{
    [Display("Content type"), StaticDataSource(typeof(ContentTypeHandler))]
    public string ContentType { get; set; } = default!;
}