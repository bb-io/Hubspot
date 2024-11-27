using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.Emails
{
    public class CreateMarketingEmailOptionalRequest
    {
        public string? Name { get; set; }

        [StaticDataSource(typeof(LanguageHandler))]
        public string? Language { get; set; }

        [Display("Business unit ID", Description = "Business unit ID")]
        [DataSource(typeof(BusinessUnitHandler))]
        public string? BusinessUnitId { get; set; }
    }
}
