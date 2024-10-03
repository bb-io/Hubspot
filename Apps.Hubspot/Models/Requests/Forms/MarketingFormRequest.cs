using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.Forms;

public class MarketingFormRequest
{
    [Display("Form ID", Description = "The ID of the form to retrieve")]
    public string FormId { get; set; } = default!;

    public override string ToString()
    {
        return FormId;
    }
}