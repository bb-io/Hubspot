using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using static Apps.Hubspot.Actions.Content.MetaActions;

namespace Apps.Hubspot.Models.Requests;

public class FindContentTypeRequest
{
    [Display("Content ID")]
    public string ContentId { get; set; } = string.Empty;

    [Display("Content types to check")]
    [StaticDataSource(typeof(ReducedContentTypeHandler))]
    public List<string>? ContentTypes { get; set; }
}