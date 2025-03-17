namespace Apps.Hubspot.Models.Entities;

public class HtmlVariablesEntity
{
    public string PageId { get; set; } = string.Empty;
    public string PageType { get; set; } = string.Empty;
    public bool ChangeHref => !string.IsNullOrEmpty(PageId) || PageId != "0";
}