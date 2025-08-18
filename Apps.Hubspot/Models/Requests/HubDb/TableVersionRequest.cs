using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.HubDb
{
    public class TableVersionRequest
    {
        [Display("Table version")]
        [StaticDataSource(typeof(TableVersionHandler))]
        public string Version { get; set; }
    }
}
