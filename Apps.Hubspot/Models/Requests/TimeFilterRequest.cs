using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class TimeFilterRequest
{
    [Display("Created at")]
    [JsonProperty("createdAt")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }
    
    [Display("Created after")]
    [JsonProperty("createdAfter")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedAfter { get; set; }
    
    [Display("Created before")]
    [JsonProperty("createdBefore")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated at")]
    [JsonProperty("updatedAt")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAt { get; set; }
    
    [Display("Updated after")]
    [JsonProperty("updatedAfter")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAfter { get; set; }
    
    [Display("Updated before")]
    [JsonProperty("updatedBefore")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedBefore { get; set; }    
}