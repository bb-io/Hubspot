using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Requests
{
    public class PublishSitePageRequest
    {
        [DataSource(typeof(SitePageHandler))]
        public string Id { get; set; }
        public Nullable<DateTime> DateTime { get; set; }
    }
}
