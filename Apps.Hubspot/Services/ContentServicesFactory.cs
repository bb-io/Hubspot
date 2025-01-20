using Apps.Hubspot.Constants;
using Apps.Hubspot.Services.ContentServices;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Hubspot.Services;

public class ContentServicesFactory(InvocationContext invocationContext)
{
    public List<IContentService> GetContentServices(IEnumerable<string> contentTypes)
    {
        var contentServices = new List<IContentService>();
        foreach (var contentType in contentTypes)
        {
            contentServices.Add(GetContentService(contentType));
        }

        return contentServices;
    }
    
    public IContentService GetContentService(string contentType)
    {
        return contentType switch
        {
            ContentTypes.Blog => new BlogPostService(invocationContext),
            ContentTypes.LandingPage => new LandingPageService(invocationContext),
            ContentTypes.Email => new MarketingEmailService(invocationContext),
            ContentTypes.Form => new MarketingFormService(invocationContext),
            ContentTypes.SitePage => new SitePageService(invocationContext),
            _ => throw new Exception($"Unexpected content type received: {contentType}")
        };
    }
}