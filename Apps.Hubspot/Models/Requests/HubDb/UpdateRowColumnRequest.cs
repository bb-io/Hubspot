using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.HubDb;
public class UpdateRowColumnRequest
{
    [Display("Row ID")]
    public string RowId { get; set; }

    [Display("Column name")]
    public string ColumnName { get; set; }

    [Display("Text value")]
    public string? StringValue { get; set; }

    [Display("Numeric value")]
    public double? NumericValue { get; set; }

    [Display("Date value")]
    public DateTime? DateValue { get; set; }
}
