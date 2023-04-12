using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Hubspot.Connections
{
    public class OAuth2ConnectionDefinition : IConnectionDefinition
    {
        private const string ApiKeyName = "hapikey";

        public string Name => "OAuth2";

        public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
        {
            new ConnectionPropertyGroup
            {
                Name = "OAuth2",
                AuthenticationType = ConnectionAuthenticationType.OAuth2,
                ConnectionUsage = ConnectionUsage.Actions,
                ConnectionProperties = new List<ConnectionProperty>()
                {
                    new ConnectionProperty("client_id"),
                    new ConnectionProperty("client_secret"),
                    new ConnectionProperty("scope")                    
                }
            },
            new ConnectionPropertyGroup
            {
                Name = "Developer API Token",
                AuthenticationType = ConnectionAuthenticationType.Undefined,
                ConnectionUsage = ConnectionUsage.Webhooks,
                ConnectionProperties = new List<ConnectionProperty>()
                {
                    new ConnectionProperty("appId"),
                    new ConnectionProperty(ApiKeyName)
                }
            }
        };

        public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProvider(Dictionary<string, string> values)
        {
            var token = values.First(v => v.Key == "access_token");
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.Header,
                "Authorization",
                $"Bearer {token.Value}"
            );
            var apiKey = values.First(v => v.Key == ApiKeyName);
            yield return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.QueryString,
                ApiKeyName,
                token.Value
            );
        }
    }
}