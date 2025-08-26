using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests.Emails
{
    public class UpdateMarketingEmailRequest
    {
        [Display("Marketing email ID")]
        [DataSource(typeof(MarketingEmailDataHandler))]
        public string MarketingEmailId { get; set; } = default!;

        [Display("Title")]
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("subject")]
        public string? Subject { get; set; }

        [Display("Language")]
        [JsonProperty("language")]
        public string? Language { get; set; }

        [Display("State")]
        [JsonProperty("state")]
        public string? State { get; set; }

        [Display("Business unit ID")]
        [DataSource(typeof(BusinessUnitHandler))]
        [JsonProperty("businessUnitId")]
        public string? BusinessUnitId { get; set; }
    }
}
