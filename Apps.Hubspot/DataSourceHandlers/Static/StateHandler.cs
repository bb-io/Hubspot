using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers.Static;

public class StateHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("DRAFT", "Draft"),
            new("DRAFT_AB", "Draft AB"),
            new("DRAFT_AB_VARIANT", "Draft AB variant"),
            new("LOSER_AB_VARIANT", "Loser AB variant"),
            new("SCHEDULED_AB", "Scheduled AB"),
            new("PUBLISHED_OR_SCHEDULED","Published or scheduled"),
            new("PUBLISHED_AB","Published AB"),
            new("PUBLISHED_AB_VARIANT", "Published AB variant"),
        };

    }
}