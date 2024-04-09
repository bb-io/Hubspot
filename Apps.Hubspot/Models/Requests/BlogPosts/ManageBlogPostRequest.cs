using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class ManageBlogPostRequest
{
    [Display("Slug")]
    public string? Slug { get; set; }

    [Display("Content group ID")]
    public string? ContentGroupId { get; set; }

    [Display("Campaign")]
    public string? Campaign { get; set; }

    [Display("Category ID")]
    public string? CategoryId { get; set; }

    [Display("State")]
    public string? State { get; set; }

    [Display("Name")]
    public string? Name { get; set; }

    [Display("MAB experiment ID")]
    public string? MabExperimentId { get; set; }

    [Display("Archived")]
    public bool? Archived { get; set; }

    [Display("Author name")]
    public string? AuthorName { get; set; }

    [Display("AB test ID")]
    public string? AbTestId { get; set; }

    [Display("Domain")]
    public string? Domain { get; set; }

    [Display("AB status")]
    public string? AbStatus { get; set; }

    [Display("Folder ID")]
    public string? FolderId { get; set; }

    [Display("HTML title")]
    public string? HtmlTitle { get; set; }

    [Display("Post body")]
    public string? PostBody { get; set; }

    [Display("Post summary")]
    public string? PostSummary { get; set; }

    [Display("RSS body")]
    public string? RssBody { get; set; }

    [Display("RSS summary")]
    public string? RssSummary { get; set; }

    [Display("Head HTML")]
    public string? HeadHtml { get; set; }

    [Display("Footer HTML")]
    public string? FooterHtml { get; set; }
    
    [Display("Meta description")]
    public string? MetaDescription { get; set; }
}