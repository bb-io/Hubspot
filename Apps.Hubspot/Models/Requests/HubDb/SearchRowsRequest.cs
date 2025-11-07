using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.HubDb;

public class SearchRowsRequest : TableIdentifier
{
    [Display("Filter query")]
    public string? FilterQuery { get; set; } // e.g. "column1__gt=5"

    [Display("Updated after")]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    public DateTime? UpdatedBefore { get; set; }
}



