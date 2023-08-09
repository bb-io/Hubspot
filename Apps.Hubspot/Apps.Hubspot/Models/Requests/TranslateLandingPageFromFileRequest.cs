using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests
{
	public class TranslateLandingPageFromFileRequest
	{
		public byte[] File { get; set; }
		public string TargetLanguage { get; set; }
        [DataSource(typeof(LandingPageHandler))]
        public string SourcePageId { get; set; }
	}


}

