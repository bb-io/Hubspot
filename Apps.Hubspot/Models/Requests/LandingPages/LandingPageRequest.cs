using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.LandingPages;

public class LandingPageRequest
{
    [Display("Landing page ID")]
    [DataSource(typeof(LandingPageHandler))]
    public string PageId { get; set; }
}