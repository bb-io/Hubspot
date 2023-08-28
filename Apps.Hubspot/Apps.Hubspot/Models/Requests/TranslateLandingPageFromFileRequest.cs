using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Hubspot.Models.Requests
{
	public class TranslateLandingPageFromFileRequest
	{
        [Display("File")]
        public File File { get; set; }
		public string TargetLanguage { get; set; }
        [DataSource(typeof(LandingPageHandler))]
		[Display("Source page")]
        public string SourcePageId { get; set; }
	}


}

