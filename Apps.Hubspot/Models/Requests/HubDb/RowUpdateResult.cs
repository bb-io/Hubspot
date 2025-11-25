
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.HubDb;

public class RowUpdateResult
{
    [Display("Original row ID")]
    public string OriginalRowId { get; set; }

    [Display("New row ID")]
    public string NewRowId { get; set; }
    public string Action { get; set; } // "created" or "updated"
}
