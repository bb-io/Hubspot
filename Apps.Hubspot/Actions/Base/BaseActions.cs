using Apps.Hubspot.Invocables;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Hubspot.Actions.Base;

public class BaseActions : HubSpotInvocable
{
    protected readonly IFileManagementClient FileManagementClient;
    
    protected BaseActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
        : base(invocationContext)
    {
        FileManagementClient = fileManagementClient;
    }
}