//using Blackbird.Applications.Sdk.Common.Authentication;
//using Blackbird.Applications.Sdk.Common.Connections;

//namespace Apps.Hubspot.Connections
//{
//    internal class DeveloperApiKeyTokenConnectionDefinition : IConnectionDefinition
//    {
//        private const string ApiKeyName = "hapikey";

//        public string Name => "Developer API Key Token";

//        public ConnectionAuthenticationType AuthenticationType => ConnectionAuthenticationType.Undefined;

//        public IEnumerable<ConnectionProperty> ConnectionProperties => new[] { new ConnectionProperty(ApiKeyName), new ConnectionProperty("appId") };

//        public AuthenticationCredentialsProvider CreateAuthorizationCredentialsProvider(Dictionary<string, string> values)
//        {
//            var token = values.First(v => v.Key == ApiKeyName);
//            return new AuthenticationCredentialsProvider(
//                AuthenticationCredentialsRequestLocation.QueryString,
//                ApiKeyName,
//                token.Value
//            );
//        }
//    }
//}
