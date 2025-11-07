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

    [DefinitionIgnore]
    public Dictionary<string, object> Values { get; set; }
    public string? Path { get; set; }
    public string? Name { get; set; }

    [Display("Child table ID")]
    public string? ChildTableId { get; set; }

    [Display("Row values")]
    public IEnumerable<string>? FlattenedValues
    {
        get
        {
            if (Values == null) return Enumerable.Empty<string>();
            return Values.Values.Select(v => v?.ToString() ?? string.Empty);
        }
    }
}
