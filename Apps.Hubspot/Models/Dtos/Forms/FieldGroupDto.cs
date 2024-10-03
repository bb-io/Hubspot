using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Dtos.Forms;

public record FieldGroupDto
{
    [Display("Group type"), JsonProperty("groupType")]
    public string GroupType { get; set; } = default!;
    
    [Display("Rich text type"), JsonProperty("richTextType")]
    public string RichTextType { get; set; } = default!;
    
    [JsonProperty("fields")]
    public List<FieldDto> Fields { get; set; } = new();
}

public record FieldDto
{
    [Display("Object type ID"), JsonProperty("objectTypeId")]
    public string ObjectTypeId { get; set; } = default!;
    
    [Display("Name"), JsonProperty("name")]
    public string Name { get; set; } = default!;
    
    [Display("Label"), JsonProperty("label")]
    public string Label { get; set; } = default!;
    
    [Display("Required"), JsonProperty("required")]
    public bool Required { get; set; }
    
    [Display("Hidden"), JsonProperty("hidden")]
    public bool Hidden { get; set; }
    
    [Display("Placeholder"), JsonProperty("placeholder")]
    public string Placeholder { get; set; } = default!;
    
    [Display("Description"), JsonProperty("description")]
    public string Description { get; set; } = default!;
    
    [Display("FieldType"), JsonProperty("fieldType")]
    public string FieldType { get; set; } = default!;

    [JsonProperty("options")]
    public List<OptionDto>? Options { get; set; }

    [JsonProperty("validation"), DefinitionIgnore]
    public Dictionary<string, object> Validation { get; set; } = new();
}

public record OptionDto
{
    [Display("Value"), JsonProperty("value")]
    public string Value { get; set; } = default!;
    
    [Display("Label"), JsonProperty("label")]
    public string Label { get; set; } = default!;
    
    [Display("Description"), JsonProperty("description")]
    public string Description { get; set; } = default!;

    [Display("Display order"), JsonProperty("displayOrder")]
    public int DisplayOrder { get; set; }
}