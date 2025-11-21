using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.HubDb;

public class UploadRowsContentRequest
{
    public FileReference File { get; set; }

    [Display("Behavior")]
    [StaticDataSource(typeof(CreateOrUpdateDataSource))]
    public string Behavior { get; set; } // "create" or "update"
}
