using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class SitePageHandler : HubSpotInvocable, IAsyncDataSourceHandler
{
    public SitePageHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var endpoint = $"/pages/site-pages?name__icontains={context.SearchString}&translatedFromId__is_null&limit=20";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        
        var sitePages = await Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
        
        return sitePages.Results
            .ToDictionary(k => k.Id, v => v.Name);
    }
}