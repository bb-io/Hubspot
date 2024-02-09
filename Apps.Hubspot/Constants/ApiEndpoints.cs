namespace Apps.Hubspot.Constants;

public static class ApiEndpoints
{
    public const string BlogPostsSegment = "/blogs/posts";
    private const string PagesSegment = "/pages";
    private const string LandingPagesSegment = "landing-pages";
    private const string SitePagesSegment = "site-pages";
    public const string MarketingEmailsEndpoint = "https://api.hubapi.com/marketing/v3/emails/";

    public static readonly string CreateTranslation = $"{SitePages()}/multi-language/create-language-variation";
    public static readonly string PublishPage = $"{SitePages()}/schedule";
    public static readonly string PublishLandingPage = $"{LandingPages()}/schedule";

    public static readonly string CreateLandingPageTranslation =
        $"{LandingPages()}/multi-language/create-language-variation";

    public static string BaseByPages(string pageType) => $"{PagesSegment}/{pageType}";
    public static string LandingPages(DateTime? dateTime = null) => BaseWithQueryString(LandingPagesSegment, dateTime);
    public static string ALandingPage(string landingPageId) => $"{LandingPages()}/{landingPageId}";
    public static string UpdateLandingPage(string sitePageId) => $"{LandingPages()}/{sitePageId}";
    public static string SitePages(DateTime? dateTime = null) => BaseWithQueryString(SitePagesSegment, dateTime);
    public static string ASitePage(string sitePageId) => $"{SitePages()}/{sitePageId}";
    public static string UpdatePage(string sitePageId) => $"{SitePages()}/{sitePageId}";

    public static string BaseWithQueryString(string pageType, DateTime? dateTime) =>
        (dateTime == null ? $"{BaseByPages(pageType)}" : $"{BaseByPages(pageType)}?updatedAfter={dateTime.ToString()}");
}