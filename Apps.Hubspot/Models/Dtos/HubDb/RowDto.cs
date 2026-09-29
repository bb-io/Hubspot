using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Dtos.HubDb;

public class RowDto
{
    [Display("Row ID")]
    public string Id { get; set; }
    
    [Display("Created at")]
    public DateTime? CreatedAt { get; set; }

    [Display("Updated at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonProperty("values")]
    private Dictionary<string, object>? Values { get; set; }
    public string? Path { get; set; }
    public string? Name { get; set; }

    [Display("Child table ID")]
    public string? ChildTableId { get; set; }

    [Display("Row values")]
    public IEnumerable<string>? FlattenedValues
    {
        get
        {
            return Values?.Values.Select(v => v?.ToString() ?? string.Empty)
                ?? Enumerable.Empty<string>();
        }
    }

    public bool TryGetColumnValue(string columnName, out object? value)
    {
        if (Values == null)
        {
            value = null;
            return false;
        }

        return Values.TryGetValue(columnName, out value);
    }
}
