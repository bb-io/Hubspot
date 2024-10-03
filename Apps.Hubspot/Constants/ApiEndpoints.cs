﻿namespace Apps.Hubspot.Constants;

public static class ApiEndpoints
{
    private const string PagesSegment = "/pages";
    private const string LandingPagesSegment = "landing-pages";
    private const string SitePagesSegment = "site-pages";
    
    public const string MarketingEmailsEndpoint = "https://api.hubapi.com/marketing/v3/emails/";
    
    public const string MarketingFormsEndpoint = "https://api.hubapi.com/marketing/v3/forms";

    public const string BlogPostsSegment = "/blogs/posts";
    public const string SitePages = $"{PagesSegment}/{SitePagesSegment}";
    public const string LandingPages = $"{PagesSegment}/{LandingPagesSegment}";

    public const string CreateTranslation = $"{SitePages}/multi-language/create-language-variation";
    public const string PublishPage = $"{SitePages}/schedule";
    public const string PublishLandingPage = $"{LandingPages}/schedule";
    public const string CreateLandingPageTranslation = $"{LandingPages}/multi-language/create-language-variation";

    public static string ALandingPage(string landingPageId) => $"{LandingPages}/{landingPageId}";
    public static string UpdateLandingPage(string sitePageId) => $"{LandingPages}/{sitePageId}";
    public static string ASitePage(string sitePageId) => $"{SitePages}/{sitePageId}";
    public static string UpdatePage(string sitePageId) => $"{SitePages}/{sitePageId}";
}