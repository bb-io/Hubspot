using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using System.Globalization;

namespace Apps.Hubspot.Auth.OAuth2;

public class OAuth2TokenService(InvocationContext invocationContext)
    : BaseInvocable(invocationContext), IOAuth2TokenService, ITokenRefreshable
{
    public bool IsRefreshToken(Dictionary<string, string> values)
    {
        var expiresAt = DateTime.Parse(values[CredsNames.ExpiresAt]);
        return DateTime.UtcNow > expiresAt;
    }

    public int? GetRefreshTokenExprireInMinutes(Dictionary<string, string> values)
    {
        if (!values.TryGetValue(CredsNames.ExpiresAt, out var expireValue))
            return null;

        if (!DateTime.TryParse(expireValue, out var expireDate))
            return null;

        var difference = expireDate - DateTime.UtcNow;

        return (int)difference.TotalMinutes - 5;
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
        var tokenDto = JsonConvert.DeserializeObject<HubSpotTokenDto>(responseContent!)!;

        var customExpiresIn = Math.Max(0, tokenDto.ExpiresIn - 10);
        var expiresAt = DateTime.UtcNow.AddSeconds(customExpiresIn);
        var expiresAtStr = expiresAt.ToString("o", CultureInfo.InvariantCulture);
        
        return new Dictionary<string, string>
        {
            { CredsNames.AccessToken, tokenDto.AccessToken },
            { CredsNames.RefreshToken, tokenDto.RefreshToken ?? string.Empty },
            { CredsNames.ExpiresAt, expiresAtStr },
            { "token_type", tokenDto.TokenType ?? string.Empty },
            { "hub_id", tokenDto.HubId?.ToString() ?? string.Empty }
        };
    }

    private async Task<string> ExecuteTokenRequest(Dictionary<string, string> parameters,
        CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        using var content = new FormUrlEncodedContent(parameters);
        using var response = await client.PostAsync(Urls.Token, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Error requesting token: {response.StatusCode} - {errorContent}");
        }

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}