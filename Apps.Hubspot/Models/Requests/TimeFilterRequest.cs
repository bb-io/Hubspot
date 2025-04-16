using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests;

public class TimeFilterRequest
{
    [Display("Created at")]
    [JsonProperty("createdAt__eq")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? CreatedAt { get; set; }
    
    [Display("Created after")]
    [JsonProperty("createdAt__gt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? CreatedAfter { get; set; }

    [Display("Created before")]
    [JsonProperty("createdAt__lt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? CreatedBefore { get; set; }

    [Display("Updated at")]
    [JsonProperty("updatedAt__eq")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? UpdatedAt { get; set; }

    [Display("Updated after")]
    [JsonProperty("updatedAt__gt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? UpdatedAfter { get; set; }

    [Display("Updated before")]
    [JsonProperty("updatedAt__lt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? UpdatedBefore { get; set; }

    [Display("Published at")]
    [JsonProperty("publishDate__eq")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? PublishedAt { get; set; }

    [Display("Published after")]
    [JsonProperty("publishDate__gt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? PublishedAfter { get; set; }

    [Display("Published before")]
    [JsonProperty("publishDate__lt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? PublishedBefore { get; set; }

    [Display("Archived at")]
    [JsonProperty("archivedAt__eq")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? ArchivedAt { get; set; }

    [Display("Archived after")]
    [JsonProperty("archivedAt__gt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? ArchivedAfter { get; set; }

    [Display("Archived before")]
    [JsonProperty("archivedAt__lt")]
    [JsonConverter(typeof(DateTimeToUnixEpoch))]
    public DateTime? ArchivedBefore { get; set; }

}