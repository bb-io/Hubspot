using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Emails;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class MarketingEmailDataHandler(InvocationContext invocationContext)
    : HubSpotInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Get, Creds);
        var result = await Client.Paginate<MarketingEmailDto>(request);

        return result
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.CreatedAt)
            .Take(30)
            .Select(x=> new DataSourceItem
            {
                Value= x.Id,
                DisplayName= x.Name
            });
    }
}