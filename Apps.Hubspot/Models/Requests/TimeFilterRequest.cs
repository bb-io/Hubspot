using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class TimeFilterRequest
{
    [Display("Created at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? CreatedAt { get; set; }
    
    [Display("Created after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? CreatedAfter { get; set; }
    
    [Display("Created before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; }

    [Display("Updated after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Published at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? PublishedAt { get; set; }

    [Display("Published after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? PublishedAfter { get; set; }

    [Display("Published before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? PublishedBefore { get; set; }

    [Display("Archived at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? ArchivedAt { get; set; }

    [Display("Archived after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? ArchivedAfter { get; set; }

    [Display("Archived before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [JsonIgnore]
    public DateTime? ArchivedBefore { get; set; }

}