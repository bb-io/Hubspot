using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class LandingPageHandler(InvocationContext invocationContext)
    : HubSpotInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var endpoint = $"/pages/landing-pages?name__icontains={context.SearchString}&translatedFromId__is_null&limit=20";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var landingPages = await Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);

        return landingPages.Results.Select(lp => new DataSourceItem
        {
            Value = lp.Id,
            DisplayName = lp.Name
        });
    }
}