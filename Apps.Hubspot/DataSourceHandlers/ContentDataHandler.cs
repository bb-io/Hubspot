using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Services;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Hubspot.DataSourceHandlers;

public class ContentDataHandler(
    InvocationContext invocationContext,
    [ActionParameter] ContentTypeFilter contentTypeFilter)
    : HubSpotInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    private readonly ContentServicesFactory _factory = new(invocationContext);

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(contentTypeFilter.ContentType))
        {
            throw new PluginMisconfigurationException("Please select 'Content type' input first");
        }
        
        var filters = new Dictionary<string, string>
        {
            { "limit", "20" }
        };

        if (!string.IsNullOrEmpty(context.SearchString))
        {
            filters.Add("name__icontains", context.SearchString);
        }

        var contentService = _factory.GetContentService(contentTypeFilter.ContentType);
        var results = await contentService.SearchContentAsync(filters, new());

        return results
            .Where(x => context.SearchString == null || x.Title.Contains(context.SearchString))
            .Select(lp => new DataSourceItem { Value = lp.ContentId, DisplayName = lp.Title })
            .ToList();
    }
}