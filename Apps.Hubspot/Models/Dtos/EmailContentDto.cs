using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos;

public class EmailContentDto
{
    public string Name { get; set; }
    
    public string Language { get; set; }
    
    public JObject Content { get; set; }
    public string BusinessUnitId { get; set; }
}