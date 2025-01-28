using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Hubspot.Connections;

public class OAuth2ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
    {
        new()
        {
            Name = "OAuth2",
            AuthenticationType = ConnectionAuthenticationType.OAuth2,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.BusinessUnit)
                {
                    DisplayName = "Business unit needed",
                    Description = "Specify this field as true if you need access to the business unit inside the app",
                    DataItems =
                    [
                        new("false", "False"),
                        new("true", "True")
                    ]
                }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        return values.Select(x =>
            new AuthenticationCredentialsProvider(
                x.Key,
                x.Value));
    }
}