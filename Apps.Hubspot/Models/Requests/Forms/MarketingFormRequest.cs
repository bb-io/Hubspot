using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.Forms;

public class MarketingFormRequest
{
    [Display("Form ID", Description = "The ID of the form to retrieve"), DataSource(typeof(MarketingFormDataHandler))]
    public string FormId { get; set; } = default!;

    public override string ToString()
    {
        return FormId;
    }
}