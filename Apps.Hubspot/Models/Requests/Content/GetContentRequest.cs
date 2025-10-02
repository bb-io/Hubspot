using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Hubspot.Models.Requests.Content;

public class GetContentRequest : ContentTypeFilter, IDownloadContentInput
{
    [Display("Content ID"), DataSource(typeof(ContentDataHandler))]
    public string ContentId { get; set; } = string.Empty;
}