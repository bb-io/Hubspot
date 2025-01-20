using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.DataSourceHandlers.Static;

internal class ContentTypeHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new DataSourceItem("blog", "Blog posts"),
            new DataSourceItem("landing_page", "Landing pages"),
            new DataSourceItem("site_page", "Site pages"),
            new DataSourceItem("email", "Marketing emails"),
            new DataSourceItem("form", "Marketing forms")
        };

    }
}
