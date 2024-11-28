using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Requests.Forms;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class MarketingFormDataHandler(InvocationContext invocationContext)
    : HubSpotInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var input = new SearchFormsRequest { FormTypes = new[] { "all" } };
        var request = new HubspotRequest($"{ApiEndpoints.MarketingFormsEndpoint}" + input.BuildQuery(), Method.Get, Creds);

        var result = await Client.Paginate<MarketingFormDto>(request);
        return result
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DataSourceItem
            {
                Value=x.Id,
                DisplayName=x.Name
            });
    }
}