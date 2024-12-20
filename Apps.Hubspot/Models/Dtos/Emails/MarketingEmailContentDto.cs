using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos.Emails
{
    public class MarketingEmailContentDto
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public string Subject { get; set; }
        public bool SendOnPublish { get; set; }
        public bool Archived { get; set; }
        public string Language { get; set; }
        public string ActiveDomain { get; set; }
        public DateTime? PublishDate { get; set; }
        public string BusinessUnitId { get; set; }
        public JObject Content { get; set; }
        public string HtmlVersion { get; set; }
    }
}
