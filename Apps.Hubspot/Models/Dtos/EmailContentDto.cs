using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos;

public class EmailContentDto
{
    public string Name { get; set; } = string.Empty;
    
    public string Language { get; set; } = string.Empty;
    
    public JObject Content { get; set; } = new();

    public string BusinessUnitId { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;
}