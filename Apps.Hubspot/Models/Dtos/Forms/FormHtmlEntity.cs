namespace Apps.Hubspot.Models.Dtos.Forms;

public record FormHtmlEntity(string Name, Dictionary<string, string> Properties, Dictionary<string, string>? Options, string FieldType);

public record FormHtmlEntities(string FormName, List<FormHtmlEntity> FieldGroups);