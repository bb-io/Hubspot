using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.LandingPages;

public class PublishLandingPageRequest
{
	[Display("Landing page ID")]
	[DataSource(typeof(LandingPageHandler))]
	public string Id { get; set; }
	
	[Display("Date time")]
	public DateTime? DateTime { get; set; }
}