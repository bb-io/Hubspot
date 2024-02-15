using Apps.Hubspot.Utils.Converters;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests.SitePages;

public class PublishPageRequest
{
    public string Id { get; set; }
    
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime PublishDate { get; set; }
}