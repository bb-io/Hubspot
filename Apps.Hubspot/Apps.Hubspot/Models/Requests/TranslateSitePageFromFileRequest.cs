using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Requests
{
    public class TranslateSitePageFromFileRequest
    {
        public byte[] File { get; set; }
        public string TargetLanguage { get; set; }
        [DataSource(typeof(SitePageHandler))]
        public string SourcePageId { get; set; }
    }
}
