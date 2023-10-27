using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Utils.Extensions.String;

namespace Apps.Hubspot.Auth.OAuth2;

public class OAuth2AuthorizeService : IOAuth2AuthorizeService
{
    public string GetAuthorizationUrl(Dictionary<string, string> values)
    {
        string bridgeOauthUrl = $"{ApplicationConstants.BridgeUri}/oauth";
        var parameters = new Dictionary<string, string>
        {
            { "client_id", ApplicationConstants.ClientId },
            { "redirect_uri", $"{ApplicationConstants.BridgeUri.TrimEnd('/')}/AuthorizationCode" },
            { "scope", ApplicationConstants.Scope },
            { "state", values["state"] },
            { "authorization_url", Urls.OAuth},
            { "actual_redirect_uri", ApplicationConstants.RedirectUri },
        };
        
        return Urls.OAuth.WithQuery(parameters);
    }
}