using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Connections
{
    public class BearerConnectionProvider : IConnectionProvider
    {
        const string AuthKeyName = "Authorization";

        public string ConnectionName => "OAuth";
        public IEnumerable<string> ConnectionProperties => new[] { "Authorization" };

        public AuthenticationCredentialsProvider Create(IDictionary<string, string> connectionValues)
        {
            var token = connectionValues.First(v => v.Key == AuthKeyName);
            return new AuthenticationCredentialsProvider(
                AuthenticationCredentialsRequestLocation.Header,
                token.Key,
                token.Value
            );
        }
    }
}