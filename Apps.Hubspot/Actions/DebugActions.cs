using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Hubspot.Actions
{
    [ActionList] public class DebugActions(InvocationContext invocationContext) : BaseInvocable(invocationContext) {
        [Action("Debug", Description = "Debug action")]
        public List<AuthenticationCredentialsProvider> DebugAction()
        {
            return InvocationContext.AuthenticationCredentialsProviders.ToList();
        }
    }
}
