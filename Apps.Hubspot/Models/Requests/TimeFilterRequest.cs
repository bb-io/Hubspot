using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class TimeFilterRequest
{
    [Display("Created at")]
    [JsonProperty("createdAt__eq")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }
    
    [Display("Created after")]
    [JsonProperty("createdAt_gte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedAfter { get; set; }
    
    [Display("Created before")]
    [JsonProperty("createdAt_lte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated at")]
    [JsonProperty("updatedAt_eq")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAt { get; set; }
    
    [Display("Updated after")]
    [JsonProperty("updatedAt_gte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAfter { get; set; }
    
    [Display("Updated before")]
    [JsonProperty("updatedAt_lte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedBefore { get; set; }    
}