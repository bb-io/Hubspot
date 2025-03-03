using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class DomainDataHandler(InvocationContext invocationContext) : HubSpotInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var request = new HubspotRequest("/domains", Method.Get, Creds);
        if (!string.IsNullOrEmpty(context.SearchString))
        {
            request.AddParameter("domain", context.SearchString, ParameterType.QueryString);
        }

        var landingPages = await Client.ExecuteWithErrorHandling<GetAllResponse<DomainDto>>(request);
        return landingPages.Results.Select(domain => new DataSourceItem(domain.Id, domain.Domain));
    }
}