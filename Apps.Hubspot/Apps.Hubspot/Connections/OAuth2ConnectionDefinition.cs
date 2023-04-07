using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Hubspot.Connections
{
    public class OAuth2ConnectionDefinition : IConnectionDefinition
    {
        public string Name => "OAuth2";

        public ConnectionAuthenticationType AuthenticationType => ConnectionAuthenticationType.OAuth2;

        public IEnumerable<ConnectionProperty> ConnectionProperties => new List<ConnectionProperty>()
        {
            new ConnectionProperty("client_id"),
            new ConnectionProperty("client_secret"),
            new ConnectionProperty("scope"),
            new ConnectionProperty("appId")
        };

        public AuthenticationCredentialsProvider CreateAuthorizationCredentialsProvider(Dictionary<string, string> values)
        {
            var token = values.First(v => v.Key == "access_token");
            return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.Header,
                AuthKeyName,
                $"Bearer {token.Value}"
            );
        }
    }
}