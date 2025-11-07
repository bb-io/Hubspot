using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.HubDb;

    public class TableExportRequest : TableIdentifier
    {

        [Display("Export format")]
        [StaticDataSource(typeof(TableExportFormatHandler))]
        public string ExportFormat { get; set; }
    }


