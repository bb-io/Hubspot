using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class TranslationInfoDto
    {
        public bool ArchivedInDashboard { get; set; }
        public DateTime Created { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public bool PublicAccessRulesEnabled { get; set; }
        public DateTime PublishDate { get; set; }
        public string Slug { get; set; }
        public string State { get; set; }
        public List<long> TagIds { get; set; }
        public DateTime Updated { get; set; }
    }
}
