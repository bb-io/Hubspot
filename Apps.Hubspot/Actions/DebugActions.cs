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
    [Action("Debug", Description = "Debug action")]
    public List<AuthenticationCredentialsProvider> DebugAction()
    {
        return InvocationContext.AuthenticationCredentialsProviders.ToList();
    }
    
    [Action("Get account info", Description = "Retrieves account information for the connected Hubspot account. Useful only for debugging purposes.")]
    public async Task<AccountInfoResponse> GetAccountInformation()
    {
        var response = await GetAccountInfo();
        return response;
    }
}