using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Dtos.Pages;

public class PageDto : IEquatable<PageDto>
{
    
    [Display("Page ID")]
    public string Id { get; set; }
    
    [Display("HTML title")]
    public string HtmlTitle { get; set; }

    [Display("Language")]
    public string? Language { get; set; }

    [Display("Currently published")]
    public bool CurrentlyPublished { get; set; }
    
    [Display("Current state")]
    public string CurrentState { get; set; }
    
    [Display("Slug")]
    public string Slug { get; set; }

    [Display("Content group ID")]
    public string ContentGroupId { get; set; }

    [Display("Category ID")]
    public string CategoryId { get; set; }

    [Display("State")]
    public string State { get; set; }

    [Display("Author name")]
    public string AuthorName { get; set; }

    [Display("Created by ID")]
    public string CreatedById { get; set; }

    [Display("Updated by ID")]
    public string UpdatedById { get; set; }

    [Display("Domain")]
    public string Domain { get; set; }

    [Display("Subcategory")]
    public string Subcategory { get; set; }

    [Display("Folder ID")]
    public string FolderId { get; set; }

    [Display("Page redirected")]
    public bool PageRedirected { get; set; }

    [Display("URL")]
    public string Url { get; set; }

    [Display("Page expiry enabled")]
    public bool PageExpiryEnabled { get; set; }

    [Display("Page expiry date")]
    public int PageExpiryDate { get; set; }

    [Display("Publish date")]
    public DateTime PublishDate { get; set; }

    [Display("Created")]
    public DateTime Created { get; set; }

    [Display("Updated")]
    public DateTime Updated { get; set; }

    [Display("Archived at")]
    public DateTime ArchivedAt { get; set; }

    [Display("Meta description")]
    public string MetaDescription { get; set; }

    [Display("Head HTML")]
    public string HeadHtml { get; set; }

    [Display("Footer HTML")]
    public string FooterHtml { get; set; }

    [Display("Name")]
    public string Name { get; set; }

    [Display("Translated from ID")]
    public string? TranslatedFromId { get; set; }

    public bool Equals(PageDto other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id && HtmlTitle == other.HtmlTitle && Name == other.Name && MetaDescription == other.MetaDescription;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as PageDto);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}