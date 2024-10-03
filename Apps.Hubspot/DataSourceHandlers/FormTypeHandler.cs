using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.DataSourceHandlers;

public class FormTypeHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
    {
        return new()
        {
            { "hubspot", "Hubspot" },
            { "captured", "Captured" },
            { "flow", "Flow" },
            { "blog_comment", "Blog comment" },
            { "all", "All" }
        };
    }
}