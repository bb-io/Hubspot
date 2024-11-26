﻿using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Newtonsoft.Json;
using RestSharp;
using System.Threading;

namespace Apps.Hubspot.Auth.OAuth2;

public class OAuth2TokenService : BaseInvocable, IOAuth2TokenService
{
    public OAuth2TokenService(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public bool IsRefreshToken(Dictionary<string, string> values)
    {
        var expiresAt = DateTime.Parse(values[CredsNames.ExpiresAt]);
        return DateTime.UtcNow > expiresAt;
    }

    public Task<Dictionary<string, string>> RefreshToken(Dictionary<string, string> values, CancellationToken cancellationToken)
    {
        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "client_id", ApplicationConstants.ClientId },
            { "client_secret", ApplicationConstants.ClientSecret },
            { "refresh_token", values[CredsNames.RefreshToken] },
        };
        
        return GetToken(bodyParameters, cancellationToken);
    }

    public Task<Dictionary<string, string>> RequestToken(
        string state, 
        string code, 
        Dictionary<string, string> values, 
        CancellationToken cancellationToken)
    {
        var bodyParameters = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", ApplicationConstants.ClientId },
            { "client_secret", ApplicationConstants.ClientSecret },
            { "redirect_uri", $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}/AuthorizationCode" },
            { "code", code }
        };
        
        return GetToken(bodyParameters, cancellationToken);
    }

    public Task RevokeToken(Dictionary<string, string> values)
    {
        throw new NotImplementedException();
    }

    private async Task<Dictionary<string, string>> GetToken(Dictionary<string, string> parameters,
        CancellationToken token)
    {
        var responseContent = await ExecuteTokenRequest(parameters, token);

        var resultDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent)
                                   ?.ToDictionary(r => r.Key, r => r.Value?.ToString())
                               ?? throw new InvalidOperationException(
                                   $"Invalid response content: {responseContent}");

        var expiresIn = int.Parse(resultDictionary["expires_in"]);
        var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
        resultDictionary.Add(CredsNames.ExpiresAt, expiresAt.ToString());

        return resultDictionary;
    }

    private async Task<string> ExecuteTokenRequest(Dictionary<string, string> parameters,
        CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        using var content = new FormUrlEncodedContent(parameters);
        using var response = await client.PostAsync(Urls.Token, content, cancellationToken);

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> GetUserId(InvocationContext invocationContext)
    {
        var token = invocationContext.AuthenticationCredentialsProviders.Get(CredsNames.AccessToken);
        var restClient = new RestClient($"{Urls.User}/{token}");

        var restRequest = new HubspotRequest(string.Empty,Method.Get, invocationContext.AuthenticationCredentialsProviders);

        var response = await restClient.ExecuteAsync(restRequest);
        throw new Exception($"SOme info {response.Content}");
        var serialized = JsonConvert.DeserializeObject<UserIdInfo>(response.Content);

        var userId = serialized.UserId;

        userId.ToString();

        //if (string.IsNullOrEmpty(userId) || !userId.Any())
        //{
        //    throw new Exception($"User ID {userId}");
        //}
        

        return userId;
    }
}