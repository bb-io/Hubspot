using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class BlogPostDto
    {
        public int ArchivedAt { get; set; }
        public bool ArchivedInDashboard { get; set; }
        public string AuthorName { get; set; }
        public string BlogAuthorId { get; set; }

        [JsonConverter(typeof(StringConverter))]
        public string CategoryId { get; set; }
        public string ContentGroupId { get; set; }
        public int ContentTypeCategory { get; set; }
        public string Created { get; set; }
        public string CreatedById { get; set; }
        public string CurrentState { get; set; }
        public bool CurrentlyPublished { get; set; }
        public string Domain { get; set; }
        public bool EnableGoogleAmpOutputOverride { get; set; }
        public string FeaturedImage { get; set; }
        public string FeaturedImageAltText { get; set; }
        public string HtmlTitle { get; set; }
        public string Id { get; set; }
        public string Language { get; set; }
        public string MetaDescription { get; set; }
        public string Name { get; set; }
        public bool PageExpiryEnabled { get; set; }
        public string PostBody { get; set; }
        public string PostSummary { get; set; }
        public bool PublicAccessRulesEnabled { get; set; }
        public string PublishDate { get; set; }
        public string RssBody { get; set; }
        public string RssSummary { get; set; }
        public string Slug { get; set; }
        public string State { get; set; }
        public string TranslatedFromId { get; set; }
        public string Updated { get; set; }
        public string UpdatedById { get; set; }
        public string Url { get; set; }
        public bool UseFeaturedImage { get; set; }
    }
}
