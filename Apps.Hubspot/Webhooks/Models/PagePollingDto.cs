using Apps.Hubspot.Models.Dtos.Pages;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Webhooks.Models;

public class PagePollingDto : PageDto
{
    [Display("Event type")]
    public string EventType { get; set; } = string.Empty;

    public PagePollingDto(PageDto pageDto, string eventType)
    {
        Id = pageDto.Id;
        HtmlTitle = pageDto.HtmlTitle;
        Language = pageDto.Language;
        CurrentlyPublished = pageDto.CurrentlyPublished;
        Published = pageDto.Published;
        CurrentState = pageDto.CurrentState;
        Slug = pageDto.Slug;
        ContentGroupId = pageDto.ContentGroupId;
        CategoryId = pageDto.CategoryId;
        State = pageDto.State;
        AuthorName = pageDto.AuthorName;
        CreatedById = pageDto.CreatedById;
        UpdatedById = pageDto.UpdatedById;
        Domain = pageDto.Domain;
        Subcategory = pageDto.Subcategory;
        FolderId = pageDto.FolderId;
        PageRedirected = pageDto.PageRedirected;
        Url = pageDto.Url;
        PageExpiryEnabled = pageDto.PageExpiryEnabled;
        PageExpiryDate = pageDto.PageExpiryDate;
        PublishDate = pageDto.PublishDate;
        Created = pageDto.Created;
        Updated = pageDto.Updated;
        ArchivedAt = pageDto.ArchivedAt;
        MetaDescription = pageDto.MetaDescription;
        HeadHtml = pageDto.HeadHtml;
        FooterHtml = pageDto.FooterHtml;
        Name = pageDto.Name;
        TranslatedFromId = pageDto.TranslatedFromId;
        EventType = eventType;
    }
}
