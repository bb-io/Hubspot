using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Responses;

public class AccountInfoResponse
{
    [JsonProperty("portalId"), Display("Site ID")]
    public string SiteId { get; set; } = string.Empty;

    [Display("Time zone")]
    public string TimeZone { get; set; } = string.Empty;
    
    [Display("Account type")]
    public string AccountType { get; set; } = string.Empty;
}