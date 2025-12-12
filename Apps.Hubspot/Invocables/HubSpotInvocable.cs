using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Hubspot.Invocables;

public abstract class HubSpotInvocable(InvocationContext invocationContext) : BaseInvocable(invocationContext)
{
    protected IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    protected virtual HubspotClient Client { get; } = new();

    public async Task<string> GetUserId()
    {
        var accessToken = Creds.Get(CredsNames.AccessToken).Value;
        var request = new RestRequest($"{Urls.User}/{accessToken}");

        var response = await Client.ExecuteWithErrorHandling<UserIdInfo>(request);
        return response.UserId.ToString();
    }
    
    public async Task<AccountInfoResponse> GetAccountInfo()
    {
        var request = new HubspotRequest(string.Empty, Method.Get, Creds);
        
        var client = new HubspotClient(Urls.AccountInfo);
        var response = await client.ExecuteWithErrorHandling<AccountInfoResponse>(request);
        return response;
    }

    public async Task<ActivityInfo> GetActivityInfo()
    {
        var request = new HubspotRequest(ApiEndpoints.ActivityInfo, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ActivityInfo>(request);
        return response;
    }
}