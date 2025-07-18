﻿using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers.Static;

public class BlogPostStateHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new("DRAFT", "Draft"),
            new("SCHEDULED", "Scheduled"),
            new("PUBLISHED", "Published")
        };

    }
}