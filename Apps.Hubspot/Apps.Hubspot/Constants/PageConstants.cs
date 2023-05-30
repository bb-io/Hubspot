using System;
using System.Security.AccessControl;

namespace Apps.Hubspot.Constants
{
	public static class PageConstants
	{
		private static string BaseAddress = "/cms/v3/pages";
		private static string LANDING_PAGES = "landing-pages";
		private static string SITE_PAGES = "site-pages";

		public static Func<string, string> BaseByPages = ( string pageType)=> $"{BaseAddress}/{pageType}";

		public static Func<string, DateTime?, string> BaseWithQueryString = (string pageType, DateTime? dateTime) =>  (dateTime == null ? $"{BaseByPages(pageType)}" : $"{BaseByPages(pageType)}?updatedAfter={dateTime.ToString()}");

		public static string LandingPages(DateTime? dateTime = null)
		{
			return BaseWithQueryString(LANDING_PAGES, dateTime);
		}

		public static Func<string, string> ALandingPage = (string landingPageId) => $"{LandingPages()}/{landingPageId}";

        public static string CreateLandingPageTranslation = $"{LandingPages()}/multi-language/create-language-variation";

        public static Func<string, string> UpdateLandingPage = (string sitePageId) => $"{LandingPages()}/{sitePageId}";

        public static string PublishLandingPage = $"{LandingPages()}/schedule";


        public static string SitePages (DateTime? dateTime = null){
			return BaseWithQueryString(SITE_PAGES, dateTime);
		}

        public static Func<string, string> ASitePage = (string sitePageId) => $"{SitePages()}/{sitePageId}";

		public static string CreateTranslation = $"{SitePages()}/multi-language/create-language-variation";

		public static Func<string, string> UpdatePage = (string sitePageId) => $"{SitePages()}/{sitePageId}";

		public static string PublishPage = $"{SitePages()}/schedule";

    }
}

