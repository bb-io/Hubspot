using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Webhooks.Models;

public class BlogPostPollingDto : BlogPostDto
{
    [Display("Event type")]
    public string EventType { get; set; } = string.Empty;

    public BlogPostPollingDto(BlogPostDto original, string eventType)
    {
        Id = original.Id;
        ArchivedAt = original.ArchivedAt;
        ArchivedInDashboard = original.ArchivedInDashboard;
        AuthorName = original.AuthorName;
        BlogAuthorId = original.BlogAuthorId;
        CategoryId = original.CategoryId;
        ContentGroupId = original.ContentGroupId;
        ContentTypeCategory = original.ContentTypeCategory;
        Created = original.Created;
        CreatedById = original.CreatedById;
        CurrentState = original.CurrentState;
        CurrentlyPublished = original.CurrentlyPublished;
        Domain = original.Domain;
        EnableGoogleAmpOutputOverride = original.EnableGoogleAmpOutputOverride;
        FeaturedImage = original.FeaturedImage;
        FeaturedImageAltText = original.FeaturedImageAltText;
        HtmlTitle = original.HtmlTitle;
        Language = original.Language;
        MetaDescription = original.MetaDescription;
        Name = original.Name;
        PageExpiryEnabled = original.PageExpiryEnabled;
        PostBody = original.PostBody;
        PostSummary = original.PostSummary;
        PublicAccessRulesEnabled = original.PublicAccessRulesEnabled;
        PublishDate = original.PublishDate;
        RssBody = original.RssBody;
        RssSummary = original.RssSummary;
        Slug = original.Slug;
        State = original.State;
        Updated = original.Updated;
        TranslatedFromId = original.TranslatedFromId;
        Translations = original.Translations;
        UpdatedById = original.UpdatedById;
        Url = original.Url;
        UseFeaturedImage = original.UseFeaturedImage;
        EventType = eventType;
    }
}
