using HtmlAgilityPack;

namespace Apps.Hubspot.Models.Responses.Pages;

public class PageInfoResponse
{
	public string Title { get; set; }
	public string? Language { get; set; }
    public string? BusinessUnitId { get; set; }
    public HtmlDocument HtmlDocument { get; set; }
}