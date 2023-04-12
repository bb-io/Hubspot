using Blackbird.Applications.Sdk.Authorization.OAuth2;
using System.Text.Json;

namespace Apps.Hubspot.Authorization.OAuth2
{
    public class OAuth2TokenService : IOAuth2TokenService
    {
        private const string ExpiresAtKeyName = "expires_at";

        public bool IsRefreshToken(Dictionary<string, string> values)
        {
            var expiresAt = DateTime.Parse(values[ExpiresAtKeyName]);
            return DateTime.UtcNow < expiresAt;
        }

        public Task RefreshToken(Dictionary<string, string> values)
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<string, string?>> RequestToken(string state, string code, Dictionary<string, string> values)
        {
            const string tokenUrl = "https://api.hubapi.com/oauth/v1/token";
            const string grant_type = "authorization_code";

            var bodyParameters = new Dictionary<string, string>
            {
                { "grant_type", grant_type },
                { "client_id", values["client_id"] },
                { "client_secret", values["client_secret"] },
                { "redirect_uri", values["redirect_uri"] },
                { "code", code }
            };
            var utcNow = DateTime.UtcNow;
            using HttpClient httpClient = new HttpClient();
            using var httpContent = new FormUrlEncodedContent(bodyParameters);
            using var response = await httpClient.PostAsync(tokenUrl, httpContent);
            var responseContent = await response.Content.ReadAsStringAsync();
            var resultDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent)?.ToDictionary(r => r.Key, r => r.Value?.ToString()) 
                ?? throw new InvalidOperationException($"Invalid response content: {responseContent}");
            var expriresIn = int.Parse(resultDictionary["expires_in"]);
            var expiresAt = utcNow.AddSeconds(expriresIn);
            resultDictionary.Add(ExpiresAtKeyName, expiresAt.ToString());
            return resultDictionary;
        }

        public Task RevokeToken(Dictionary<string, string> values)
        {
            throw new NotImplementedException();
        }
    }
}
