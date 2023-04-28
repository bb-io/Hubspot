using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class CreateOrUpdateBlogPostDto
    {
        public string Slug { get; set; }
        public string ContentGroupId { get; set; }
        public string Campaign { get; set; }
        public int CategoryId { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public string MabExperimentId { get; set; }
        public bool Archived { get; set; }
        public string AuthorName { get; set; }
        public string AbTestId { get; set; }
        public string Domain { get; set; }
        public string AbStatus { get; set; }
        public string FolderId { get; set; }
        public string HtmlTitle { get; set; }
        public string PostBody { get; set; }
        public string PostSummary { get; set; }
        public string RssBody { get; set; }
        public string RssSummary { get; set; }
        public string HeadHtml { get; set; }
        public string FooterHtml { get; set; }
    }
}
