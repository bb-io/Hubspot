using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class LandingPageHandler : HubSpotInvocable, IAsyncDataSourceHandler
{
    public LandingPageHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var endpoint = $"/pages/landing-pages?name__icontains={context.SearchString}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        
        var landingPages = await Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
        
        return landingPages.Results
            .ToDictionary(k => k.Id, v => v.Name);
    }
}