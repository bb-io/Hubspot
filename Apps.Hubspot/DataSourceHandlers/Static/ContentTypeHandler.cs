﻿using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Hubspot.Constants;

namespace Apps.Hubspot.DataSourceHandlers.Static;

public class ContentTypeHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new(ContentTypes.Blog, "Blog posts"),
            new(ContentTypes.BlogAuthor, "Blog authors"),
            new(ContentTypes.BlogTag, "Blog tags"),
            new(ContentTypes.BlogComment, "Blog comments"),
            new(ContentTypes.LandingPage, "Landing pages"),
            new(ContentTypes.SitePage, "Site pages"),
            new(ContentTypes.Email, "Marketing emails"),
            new(ContentTypes.Form, "Marketing forms")
        ];
    }
}
