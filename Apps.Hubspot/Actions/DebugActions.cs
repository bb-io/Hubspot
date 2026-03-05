using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Hubspot.Actions;

[ActionList] 
public class DebugActions(InvocationContext invocationContext) : HubSpotInvocable(invocationContext) 
{
    [Action("Debug", Description = "Output connection credentials for debugging")]
    public List<AuthenticationCredentialsProvider> DebugAction()
    {
        return InvocationContext.AuthenticationCredentialsProviders.ToList();
    }
    
    [Action("Get connected site info", Description = "Get account information for the connected site for debugging")]
    public async Task<AccountInfoResponse> GetAccountInformation()
    {
        var response = await GetAccountInfo();
        return response;
    }
}
