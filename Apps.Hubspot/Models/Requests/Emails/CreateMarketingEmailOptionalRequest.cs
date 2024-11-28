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

namespace Apps.Hubspot.Models.Requests.Emails
{
    public class CreateMarketingEmailOptionalRequest
    {
        public string? Name { get; set; }

        public string? Subject { get; set; }

        [Display("Send on publish")] public bool? SendOnPublish { get; set; }

        public bool? Archived { get; set; }

        [Display("Active domain")] public string? ActiveDomain { get; set; }

        [StaticDataSource(typeof(LanguageHandler))] public string? Language { get; set; }

        [Display("Publish date")]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? PublishDate { get; set; }

        [Display("Business unit ID", Description = "Business unit ID")]
        [DataSource(typeof(BusinessUnitHandler))]
        public string? BusinessUnitId { get; set; }
    }
}
