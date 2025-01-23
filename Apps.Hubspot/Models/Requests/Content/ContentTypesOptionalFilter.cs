using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.Content;

public class ContentTypesOptionalFilter
{
    [Display("Content types"), StaticDataSource(typeof(ContentTypeHandler))]
    public IEnumerable<string>? ContentTypes { get; set; }
}