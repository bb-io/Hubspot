namespace Apps.Hubspot.Models.Requests
{
	public class TranslateFromFileRequest
	{
		public byte[] File { get; set; }
		public string TargetLanguage { get; set; }
		public string SourcePageId { get; set; }
	}
}

