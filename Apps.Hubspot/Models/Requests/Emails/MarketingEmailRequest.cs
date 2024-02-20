using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.Emails;

public class MarketingEmailRequest
{
    [Display("Marketing email ID")]
    [DataSource(typeof(MarketingEmailDataHandler))]
    public string MarketingEmailId { get; set; }
}