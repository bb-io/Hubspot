using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.SitePages;

public class SitePageRequest
{
    [Display("Site page")]
    [DataSource(typeof(SitePageHandler))]
    public string PageId { get; set; }
}