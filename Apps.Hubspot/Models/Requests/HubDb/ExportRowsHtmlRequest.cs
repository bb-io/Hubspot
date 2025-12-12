using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.HubDb;

public class ExportRowsHtmlRequest : TableIdentifier
{

    [Display("Filter query")]
    public string? FilterQuery { get; set; }

    [Display("Columns to export")]
    public List<string> Columns { get; set; }
}
