using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Posts;

public class BlogPostDto
{
    [Display("Archived at")] public int ArchivedAt { get; set; }

    [Display("Archived in dashboard")] public bool ArchivedInDashboard { get; set; }

    [Display("Author name")] public string AuthorName { get; set; }

    [Display("Blog author ID")] public string BlogAuthorId { get; set; }

    [Display("Category ID")] public string CategoryId { get; set; }

    [Display("Content group ID")] public string ContentGroupId { get; set; }

    [Display("Content type category")] public int ContentTypeCategory { get; set; }

    [Display("Created")] public string Created { get; set; }

    [Display("Created by ID")] public string CreatedById { get; set; }

    [Display("Current state")] public string CurrentState { get; set; }

    [Display("Currently published")] public bool CurrentlyPublished { get; set; }

    [Display("Domain")] public string Domain { get; set; }

    [Display("Enable google AMP output override")]
    public bool EnableGoogleAmpOutputOverride { get; set; }

    [Display("Featured image")] public string FeaturedImage { get; set; }

    [Display("Featured image alt text")] public string FeaturedImageAltText { get; set; }

    [Display("HTML title")] public string HtmlTitle { get; set; }

    [Display("ID")] public string Id { get; set; }

    [Display("Language")] public string Language { get; set; }

    [Display("Meta description")] public string MetaDescription { get; set; }

    [Display("Name")] public string Name { get; set; }

    [Display("Page expiry enabled")] public bool PageExpiryEnabled { get; set; }

    [Display("Post body")] public string PostBody { get; set; }

    [Display("Post summary")] public string PostSummary { get; set; }

    [Display("Public access rules enabled")]
    public bool PublicAccessRulesEnabled { get; set; }

    [Display("Publish date")] public string PublishDate { get; set; }

    [Display("RSS body")] public string RssBody { get; set; }

    [Display("RSS summary")] public string RssSummary { get; set; }

    [Display("Slug")] public string Slug { get; set; }

    [Display("State")] public string State { get; set; }

    [Display("Updated")] public string Updated { get; set; }
    
    [Display("Translated from ID")] public string? TranslatedFromId { get; set; }

    [Display("Updated by ID")] public string UpdatedById { get; set; }

    [Display("URL")] public string Url { get; set; }

    [Display("Use featured image")] public bool UseFeaturedImage { get; set; }
}