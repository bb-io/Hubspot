using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class TimeFilterRequest
{
    [Display("Created at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedAt { get; set; }
    
    [Display("Created after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedAfter { get; set; }
    
    [Display("Created before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAt { get; set; }

    [Display("Updated after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Published at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? PublishedAt { get; set; }

    [Display("Published after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? PublishedAfter { get; set; }

    [Display("Published before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? PublishedBefore { get; set; }

    [Display("Archived at")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ArchivedAt { get; set; }

    [Display("Archived after")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ArchivedAfter { get; set; }

    [Display("Archived before")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ArchivedBefore { get; set; }

}