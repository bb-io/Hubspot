using Apps.Hubspot.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Hubspot.Invocables;

public abstract class HubSpotInvocable : BaseInvocable
{
    protected IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;
    
    protected HubspotClient Client { get; }
    
    protected HubSpotInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();
    }
}