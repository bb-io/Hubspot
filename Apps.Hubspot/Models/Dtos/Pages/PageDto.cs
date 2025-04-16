using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Dtos.Pages;

public class PageDto
{
    [Display("Page ID")]
    public string Id { get; set; } = string.Empty;
    
    [Display("HTML title")]
    public string HtmlTitle { get; set; } = string.Empty;

    [Display("Language")]
    public string? Language { get; set; }

    [Display("Currently published")]
    public bool CurrentlyPublished { get; set; }
    
    [Display("Published")]
    public bool Published { get; set; }
    
    [Display("Current state")]
    public string CurrentState { get; set; } = string.Empty;
    
    [Display("Slug")]
    public string Slug { get; set; } = string.Empty;

    [Display("Content group ID")]
    public string ContentGroupId { get; set; } = string.Empty;

    [Display("Category ID")]
    public string CategoryId { get; set; } = string.Empty;

    [Display("State")]
    public string State { get; set; } = string.Empty;

    [Display("Author name")]
    public string AuthorName { get; set; } = string.Empty;

    [Display("Created by ID")]
    public string CreatedById { get; set; } = string.Empty;

    [Display("Updated by ID")]
    public string UpdatedById { get; set; } = string.Empty;

    [Display("Domain")]
    public string Domain { get; set; } = string.Empty;

    [Display("Subcategory")]
    public string Subcategory { get; set; } = string.Empty;

    [Display("Folder ID")]
    public string FolderId { get; set; } = string.Empty;

    [Display("Page redirected")]
    public bool PageRedirected { get; set; }

    [Display("URL")]
    public string Url { get; set; } = string.Empty;

    [Display("Page expiry enabled")]
    public bool PageExpiryEnabled { get; set; }

    [Display("Page expiry date")]
    public int PageExpiryDate { get; set; }

    [Display("Publish date"), JsonProperty("publishDate")]
    public string PublishDate { get; set; } = string.Empty;

    [Display("Created"), JsonProperty("createdAt")]
    public string Created { get; set; } = string.Empty;

    [Display("Updated"), JsonProperty("updatedAt")]
    public string Updated { get; set; } = string.Empty;

    [Display("Archived at"), JsonProperty("archivedAt")]
    public string ArchivedAt { get; set; } = string.Empty;

    [Display("Meta description")]
    public string MetaDescription { get; set; } = string.Empty;

    [Display("Head HTML")]
    public string HeadHtml { get; set; } = string.Empty;

    [Display("Footer HTML")]
    public string FooterHtml { get; set; } = string.Empty;

    [Display("Page name")]
    public string Name { get; set; } = string.Empty;

    [Display("Translated from ID"), JsonProperty("translatedFromId")]
    public string TranslatedFromId { get; set; } = string.Empty;

    [Display("Query")]
    public string? Query { get; set; }

    public PageDto DeepClone(string query)
    {
        return new PageDto
        {
            Id = Id,
            HtmlTitle = HtmlTitle,
            Language = Language != null ? string.Copy(Language) : null,
            CurrentlyPublished = CurrentlyPublished,
            Published = Published,
            CurrentState = string.Copy(CurrentState),
            Slug = string.Copy(Slug),
            ContentGroupId = string.Copy(ContentGroupId),
            CategoryId = string.Copy(CategoryId),
            State = string.Copy(State),
            AuthorName = string.Copy(AuthorName),
            CreatedById = string.Copy(CreatedById),
            UpdatedById = string.Copy(UpdatedById),
            Domain = string.Copy(Domain),
            Subcategory = string.Copy(Subcategory),
            FolderId = string.Copy(FolderId),
            PageRedirected = PageRedirected,
            Url = string.Copy(Url),
            PageExpiryEnabled = PageExpiryEnabled,
            PageExpiryDate = PageExpiryDate,
            PublishDate = string.Copy(PublishDate),
            Created = string.Copy(Created),
            Updated = string.Copy(Updated),
            ArchivedAt = string.Copy(ArchivedAt),
            MetaDescription = string.Copy(MetaDescription),
            HeadHtml = string.Copy(HeadHtml),
            FooterHtml = string.Copy(FooterHtml),
            Name = string.Copy(Name),
            TranslatedFromId = string.Copy(TranslatedFromId),
            Query = query
        };
    }
}