using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.HubDb;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class DraftTableHandler(InvocationContext invocationContext) : HubSpotInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var endpoint = $"/hubdb/tables/draft";

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<GetAllResponse<TableDto>>(request);

        var results = response.Results.Where(x => context.SearchString is null || x.Name.ToLower().Contains(context.SearchString.ToLower())).ToArray();

        return results.Select(table => new DataSourceItem (table.Id, table.Name));
    }
}
