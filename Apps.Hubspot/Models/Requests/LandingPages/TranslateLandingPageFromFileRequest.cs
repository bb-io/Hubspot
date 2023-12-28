using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.LandingPages;

public class TranslateLandingPageFromFileRequest
{
	[Display("File")]
	public FileReference File { get; set; }
	
	[Display("Target language")]
	public string TargetLanguage { get; set; }

	[Display("Source page")]
	[DataSource(typeof(LandingPageHandler))]
	public string SourcePageId { get; set; }
}