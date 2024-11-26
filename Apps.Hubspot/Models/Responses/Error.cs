using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Responses;

public class Error
{
    [JsonProperty("status")]
    public string ErrorType { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}