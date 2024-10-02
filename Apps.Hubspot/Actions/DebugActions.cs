using Apps.Hubspot.Actions.Base;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Hubspot.Actions;

[ActionList]
public class DebugActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : BasePageActions(invocationContext, fileManagementClient)
{
    [Action("[DEBUG] Get credential providers", Description = "Get all credential providers. Can be used only in development environment.")]
    public List<AuthenticationCredentialsProvider> GetCredentialProviders()
    {
        return InvocationContext.AuthenticationCredentialsProviders.ToList();
    }
}