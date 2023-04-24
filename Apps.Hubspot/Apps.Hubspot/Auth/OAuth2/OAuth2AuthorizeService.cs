﻿using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Microsoft.AspNetCore.WebUtilities;

namespace Apps.Hubspot.Authorization.OAuth2
{
    public class OAuth2AuthorizeService : IOAuth2AuthorizeService
    {
        public string GetAuthorizationUrl(Dictionary<string, string> values)
        {
            const string oauthUrl = "https://app.hubspot.com/oauth/authorize";
            var parameters = new Dictionary<string, string>
            {
                { "client_id", values["client_id"] },
                { "redirect_uri", values["redirect_uri"] },
                { "scope", values["scope"] },
                { "state", values["state"] }
            };
            return QueryHelpers.AddQueryString(oauthUrl, parameters);
        }
    }
}
