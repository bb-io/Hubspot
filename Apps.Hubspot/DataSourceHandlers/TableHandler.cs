using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Hubspot.Models.Requests.HubDb;
using System;
using Apps.Hubspot.Api;
using RestSharp;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Dtos.HubDb;

namespace Apps.Hubspot.DataSourceHandlers;

public class TableHandler : HubSpotInvocable, IAsyncDataSourceHandler
{
    private TableVersionRequest VersionRequest { get; set; }

    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    public TableHandler(InvocationContext invocationContext, [ActionParameter] TableVersionRequest versionRequest) : base(invocationContext)
    {
        VersionRequest = versionRequest;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        if (VersionRequest == null || string.IsNullOrEmpty(VersionRequest.Version))
        {
            throw new ArgumentException("Please, select the table version first");
        }

        var endpoint = VersionRequest.Version.ToLower() == "published"
            ? $"/hubdb/tables"
            : $"/hubdb/tables/draft";

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<GetAllResponse<TableDto>>(request);

        var results = response.Results.Where(x => context.SearchString is null || x.Name.ToLower().Contains(context.SearchString.ToLower())).ToArray();

        return results.ToDictionary(x => x.Id, x => x.Name);
    }
}

