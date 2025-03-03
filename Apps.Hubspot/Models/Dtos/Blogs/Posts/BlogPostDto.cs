using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Blogs.Posts;

public class BlogPostDto : IEntity
{
    [Display("Blog post ID")] 
    public string Id { get; set; } = string.Empty;
    
    [Display("Archived at")] 
    public int ArchivedAt { get; set; }

    [Display("Archived in dashboard")] 
    public bool ArchivedInDashboard { get; set; }

    [Display("Author name")] 
    public string AuthorName { get; set; } = string.Empty;

    [Display("Blog author ID")]
    public string BlogAuthorId { get; set; } = string.Empty;

    [Display("Category ID")] 
    public string CategoryId { get; set; } = string.Empty;

    [Display("Content group ID")] 
    public string ContentGroupId { get; set; } = string.Empty;

    [Display("Content type category")] 
    public int ContentTypeCategory { get; set; }

    [Display("Created")] 
    public string Created { get; set; } = string.Empty;

    [Display("Created by ID")] 
    public string CreatedById { get; set; } = string.Empty;

    [Display("Current state")] 
    public string CurrentState { get; set; } = string.Empty;

    [Display("Currently published")] 
    public bool CurrentlyPublished { get; set; }

    [Display("Domain")]
    public string Domain { get; set; } = string.Empty;

    [Display("Enable google AMP output override")]
    public bool EnableGoogleAmpOutputOverride { get; set; }

    [Display("Featured image")] 
    public string FeaturedImage { get; set; } = string.Empty;

    [Display("Featured image alt text")] 
    public string FeaturedImageAltText { get; set; } = string.Empty;

    [Display("HTML title")] 
    public string HtmlTitle { get; set; } = string.Empty;
    
    [Display("Language")] 
    public string? Language { get; set; }

    [Display("Meta description")] 
    public string MetaDescription { get; set; } = string.Empty;

    [Display("Name")] 
    public string Name { get; set; } = string.Empty;

    [Display("Page expiry enabled")] 
    public bool PageExpiryEnabled { get; set; }

    [Display("Post body")] 
    public string PostBody { get; set; } = string.Empty;

    [Display("Post summary")] 
    public string PostSummary { get; set; } = string.Empty;

    [Display("Public access rules enabled")]
    public bool PublicAccessRulesEnabled { get; set; }

    [Display("Publish date")] 
    public string PublishDate { get; set; } = string.Empty;

    [Display("RSS body")] 
    public string RssBody { get; set; } = string.Empty;

    [Display("RSS summary")] 
    public string RssSummary { get; set; } = string.Empty;

    [Display("Slug")] 
    public string Slug { get; set; } = string.Empty;

    [Display("State")] 
    public string State { get; set; } = string.Empty;

    [Display("Updated")] 
    public string Updated { get; set; } = string.Empty;
    
    [Display("Translated from ID")] 
    public string? TranslatedFromId { get; set; }

    [DefinitionIgnore]
    public Dictionary<string, ObjectWithId> Translations { get; set; } = new();

    [Display("Updated by ID")] 
    public string UpdatedById { get; set; } = string.Empty;

    [Display("URL")] 
    public string Url { get; set; } = string.Empty;

    [Display("Use featured image")] 
    public bool UseFeaturedImage { get; set; }
}