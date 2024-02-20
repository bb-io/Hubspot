using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Emails;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class MarketingEmailDataHandler : HubSpotInvocable, IAsyncDataSourceHandler
{
    public MarketingEmailDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Get, Creds);
        var result = await Client.Paginate<MarketingEmailDto>(request);

        return result
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.CreatedAt)
            .Take(30)
            .ToDictionary(x => x.Id, x => x.Name);
    }
}