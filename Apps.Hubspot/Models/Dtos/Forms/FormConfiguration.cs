using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Forms;

public record FormConfiguration
{
    [Display("Language")]
    public string Language { get; set; } = default!;
    
    [Display("Cloneable")]
    public bool Cloneable { get; set; }
    
    [Display("Post submit action")]
    public PostSubmitAction PostSubmitAction { get; set; } = default!;
    
    [Display("Editable")]
    public bool Editable { get; set; }
    
    [Display("Archivable")]
    public bool Archivable { get; set; }
    
    [Display("Recaptcha enabled")]
    public bool RecaptchaEnabled { get; set; }
    
    [Display("Notify contact owner")]
    public bool NotifyContactOwner { get; set; }
    
    [Display("Notify recipients")]
    public List<string> NotifyRecipients { get; set; } = default!;
    
    [Display("Create new contact for new email")]
    public bool CreateNewContactForNewEmail { get; set; }
    
    [Display("Pre populate known values")]
    public bool PrePopulateKnownValues { get; set; }
    
    [Display("Allow link to reset known values")]
    public bool AllowLinkToResetKnownValues { get; set; }
}

public class PostSubmitAction
{
    [Display("Type")]
    public string Type { get; set; } = default!;
    
    [Display("Value")]
    public string Value { get; set; } = default!;
}