using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Forms;

public record MarketingFormDto
{
    [Display("Form ID")]
    public string Id { get; set; } = default!;

    [Display("Form name")]
    public string Name { get; set; } = default!;

    [Display("Created at")]
    public DateTime CreatedAt { get; set; }
    
    [Display("Updated at")]
    public DateTime UpdatedAt { get; set; }

    public bool Archived { get; set; }

    [Display("Form type")]
    public string FormType { get; set; } = default!;
    
    [Display("Form configuration")]
    public FormConfiguration Configuration { get; set; } = default!;
    
    [Display("Field groups")]
    public List<FieldGroupDto> FieldGroups { get; set; } = new();
}