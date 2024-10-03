using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.Forms;

public class UpdateMarketingFormRequest
{
    public FileReference File { get; set; } = default!;

    [Display("Form ID", Description = "ID of the form to update"), DataSource(typeof(MarketingFormDataHandler))]
    public string? FormId { get; set; }
}