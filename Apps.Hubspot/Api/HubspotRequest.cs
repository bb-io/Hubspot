using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using RestSharp;

namespace Apps.Hubspot.Api;

public class HubspotRequest : RestRequest
{
    public HubspotRequest(string endpoint, Method method, IEnumerable<AuthenticationCredentialsProvider> creds) 
        : base(endpoint, method)
    {
        var token = creds.Get(CredsNames.AccessToken);

        this.AddHeader("Authorization", $"Bearer {token.Value}");
        this.AddHeader("accept", "*/*");
    }
}