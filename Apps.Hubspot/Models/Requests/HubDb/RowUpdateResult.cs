namespace Apps.Hubspot.Models.Requests.HubDb;

public class RowUpdateResult
{
    public string OriginalRowId { get; set; }
    public string NewRowId { get; set; }
    public string Action { get; set; } // "created" or "updated"
}
