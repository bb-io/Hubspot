using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Apps.Hubspot.Utils.Converters;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace Apps.Hubspot.Models.Requests.Emails
{
    public class CreateMarketingEmailOptionalRequest
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("subject")]
        public string? Subject { get; set; }

        [JsonProperty("sendOnPublish")]
        [Display("Send on publish")] public bool? SendOnPublish { get; set; }

        [JsonProperty("archived")]
        public bool? Archived { get; set; }

        [JsonProperty("activeDomain")]
        [Display("Active domain")] public string? ActiveDomain { get; set; }

        [JsonProperty("language")]
        [StaticDataSource(typeof(LanguageHandler))] public string? Language { get; set; }

        [JsonProperty("publishDate")]
        [Display("Publish date")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? PublishDate { get; set; }

        [JsonProperty("businessUnitId")]
        [Display("Business unit ID", Description = "Business unit ID")]
        [DataSource(typeof(BusinessUnitHandler))]
        public string? BusinessUnitId { get; set; }

        [JsonProperty("content")]
        public Content? Content { get; set; }
    }
    public class Content
    {
        [JsonProperty("htmlVersion")]
        public string? HtmlVersion { get; set; }
    }
}
