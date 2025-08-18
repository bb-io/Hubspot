using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

public class TableVersionBothHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("published", "Published"),
            new("draft", "Draft"),
            new("both", "Both (draft or published)")

        };
    }
}