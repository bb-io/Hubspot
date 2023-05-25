using System.Globalization;

namespace Apps.Hubspot.Models.Requests
{
	public class CreateTranslationRequest
	{
		public string Id { get; set; }
		public string Language { get; private set; }
		public string PrimaryLanguage { get; private set; }

		public void SetLanguage(CultureInfo primary, CultureInfo target)
		{
			Language = target.Name;
			PrimaryLanguage = primary.Name;
		}
	}
}

