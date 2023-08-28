using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Requests
{
    public class GetLandingPageAsHtmlRequest
    {
        [DataSource(typeof(LandingPageHandler))]
        [Display("Page")]
        public string PageId { get; set; }
    }
}
