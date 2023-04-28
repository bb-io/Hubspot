using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class BlogPostDto
    {
        public string Id { get; set; }
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
        public string CreatedById { get; set; }
        public string UpdatedById { get; set; }
        public string Domain { get; set; }
        public string AbStatus { get; set; }
        public string FolderId { get; set; }
        public string HtmlTitle { get; set; }
        public bool EnableGoogleAmpOutputOverride { get; set; }
        public bool UseFeaturedImage { get; set; }
        public string PostBody { get; set; }
        public string PostSummary { get; set; }
        public string RssBody { get; set; }
        public string RssSummary { get; set; }
        public bool CurrentlyPublished { get; set; }
        public bool PageExpiryEnabled { get; set; }
        public int PageExpiryRedirectId { get; set; }
        public string PageExpiryRedirectUrl { get; set; }
        public int PageExpiryDate { get; set; }
        public bool IncludeDefaultCustomCss { get; set; }
        public bool EnableLayoutStylesheets { get; set; }
        public bool EnableDomainStylesheets { get; set; }
        public bool PublishImmediately { get; set; }
        public string FeaturedImage { get; set; }
        public string FeaturedImageAltText { get; set; }
        public string LinkRelCanonicalUrl { get; set; }
        public string ContentTypeCategory { get; set; }
        public string MetaDescription { get; set; }
        public string HeadHtml { get; set; }
        public string FooterHtml { get; set; }
        public bool ArchivedInDashboard { get; set; }
        public bool PublicAccessRulesEnabled { get; set; }
        public string Url { get; set; }
        public string PublishDate { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public string DeletedAt { get; set; }
    }
}
