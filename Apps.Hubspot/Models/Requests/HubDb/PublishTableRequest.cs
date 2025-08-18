using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.HubDb;
public class DraftTableRequest
{
    [Display("Table ID or name")]
    [DataSource(typeof(DraftTableHandler))]
    public string TableIdOrName { get; set; }
}
