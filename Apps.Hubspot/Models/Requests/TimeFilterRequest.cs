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
    [JsonProperty("createdAt__gte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedAfter { get; set; }
    
    [Display("Created before")]
    [JsonProperty("createdAt__lte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated at")]
    [JsonProperty("updatedAt__eq")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAt { get; set; }

    [Display("Updated after")]
    [JsonProperty("updatedAt__gte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    [JsonProperty("updatedAt__lte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Published at")]
    [JsonProperty("publishDate__eq")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? PublishedAt { get; set; }

    [Display("Published after")]
    [JsonProperty("publishDate__gte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? PublishedAfter { get; set; }

    [Display("Published before")]
    [JsonProperty("publishDate__lte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? PublishedBefore { get; set; }

    [Display("Archived at")]
    [JsonProperty("archivedAt__eq")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ArchivedAt { get; set; }

    [Display("Archived after")]
    [JsonProperty("archivedAt__gte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ArchivedAfter { get; set; }

    [Display("Archived before")]
    [JsonProperty("archivedAt__lte")]
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ArchivedBefore { get; set; }

}