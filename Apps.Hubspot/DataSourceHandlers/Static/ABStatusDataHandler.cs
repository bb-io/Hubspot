using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers.Static;

public class ABStatusDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("MASTER", "Master"),
            new("VARIANT", "Variant"),
            new("LOSER_VARIANT", "Loser variant")
        };

    }
}