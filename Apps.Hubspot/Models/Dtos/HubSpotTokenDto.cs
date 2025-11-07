using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Dtos;

public class HubSpotTokenDto
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; } = default!;

    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("token_type")]
    public string? TokenType { get; set; }
    
    [JsonProperty("hub_id")]
    public long? HubId { get; set; }
    [JsonProperty("scopes")]     
     public string[]? Scopes { get; set; }
}
