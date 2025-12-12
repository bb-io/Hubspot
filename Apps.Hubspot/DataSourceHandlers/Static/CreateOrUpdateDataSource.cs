using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers.Static;

public class CreateOrUpdateDataSource : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData() =>
        new List<DataSourceItem>
        {
            new("Create new rows", "create"),
            new("Update original rows", "update")
        };
}