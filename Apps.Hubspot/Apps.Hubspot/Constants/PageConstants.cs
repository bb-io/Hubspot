using System;
namespace Apps.Hubspot.Constants
{
	public static class PageConstants
	{
		public static string BaseAddress = "/cms/v3/pages";


		public static string LandingPages = $"{BaseAddress}/landing-pages";

		public static Func<string, string> ALandingPage = (string landingPageId) => $"{LandingPages}/{landingPageId}";


		public static string SitePages = $"{BaseAddress}/site-pages";

        public static Func<string, string> ASitePage = (string sitePageId) => $"{SitePages}/{sitePageId}";

		public static string CreateTranslation = $"{SitePages}/multi-language/create-language-variation";

		public static Func<string, string> UpdatePage = (string sitePageId) => $"{SitePages}/{sitePageId}";

		public static string PublishPage = $"{SitePages}/schedule";

    }
}

