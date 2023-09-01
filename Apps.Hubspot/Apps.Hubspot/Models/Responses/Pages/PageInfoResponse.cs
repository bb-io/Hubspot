using HtmlAgilityPack;

namespace Apps.Hubspot.Models.Responses.Pages;

public class PageInfoResponse
{
	public string Title { get; set; }
	public string PageId { get; set; }
	public string Language { get; set; }
	public HtmlDocument Html { get; set; }
}