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
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Requests.Emails
{
    public class CreateMarketingEmailOptionalRequest
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("language")]
        [StaticDataSource(typeof(LanguageHandler))] public string? Language { get; set; }

        [JsonProperty("businessUnitId")]
        [Display("Business unit ID", Description = "Business unit ID")]
        [DataSource(typeof(BusinessUnitHandler))]
        public string? BusinessUnitId { get; set; }

        [JsonProperty("content")]
        public Content? Content { get; set; }
    }

    public class Content
    {
        [JsonProperty("plainTextVersion")]
        public string? PlainTextVersion { get; set; }

        [JsonProperty("templatePath")]
        public string? TemplatePath { get; set; }

        [JsonProperty("styleSettings")]
        public object? StyleSettings { get; set; }

        [JsonProperty("flexAreas")]
        public object? FlexAreas { get; set; }

        [JsonProperty("widgets")]
        public object? Widgets { get; set; }
    }
}
