using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers;

public class FormTypeHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new DataSourceItem("hubspot", "Hubspot"),
            new DataSourceItem("captured", "Captured"),
            new DataSourceItem("flow", "Flow"),
            new DataSourceItem("blog_comment", "Blog comment"),
            new DataSourceItem("all", "All")
        };
        
    }
}