using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.DataSourceHandlers.Static;

public class ContentTypeForTranslationLanguageCodesHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return new List<DataSourceItem>
        {
            new(ContentTypes.Blog, "Blog posts"),
            new(ContentTypes.LandingPage, "Landing pages"),
            new(ContentTypes.SitePage, "Site pages")
        };
    }
}