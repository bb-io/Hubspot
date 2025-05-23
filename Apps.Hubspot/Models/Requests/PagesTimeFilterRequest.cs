using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class PagesTimeFilterRequest
{
    [Display("Created at")]
    [JsonProperty("createdAt__eq")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? CreatedAt { get; set; }
    
    [Display("Created after")]
    [JsonProperty("createdAt__gt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    [JsonProperty("createdAt__lt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated at")]
    [JsonProperty("updatedAt__eq")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? UpdatedAt { get; set; }

    [Display("Updated after")]
    [JsonProperty("updatedAt__gt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    [JsonProperty("updatedAt__lt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Published at")]
    [JsonProperty("publishDate__eq")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? PublishedAt { get; set; }

    [Display("Published after")]
    [JsonProperty("publishDate__gt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? PublishedAfter { get; set; }

    [Display("Published before")]
    [JsonProperty("publishDate__lt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? PublishedBefore { get; set; }

    [Display("Archived at")]
    [JsonProperty("archivedAt__eq")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? ArchivedAt { get; set; }

    [Display("Archived after")]
    [JsonProperty("archivedAt__gt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? ArchivedAfter { get; set; }

    [Display("Archived before")]
    [JsonProperty("archivedAt__lt")]
    [JsonConverter(typeof(DateTimeToTimestamp))]
    public DateTime? ArchivedBefore { get; set; }
}