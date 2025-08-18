using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.HubDb
{
    public class TableIdentifier
    {
        [Display("Table ID or name")]
        [DataSource(typeof(TableHandler))]
        public string TableIdOrName { get; set; }
    }
}
