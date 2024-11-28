using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Dtos.Emails
{
    public class HtmlValues
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public bool? SendOnPublish { get; set; }
        public bool? Archived { get; set; }
        public string ActiveDomain { get; set; }
        public string Language { get; set; }
        public DateTime? PublishDate { get; set; }
        public string BusinessUnitId { get; set; }
    }
}
