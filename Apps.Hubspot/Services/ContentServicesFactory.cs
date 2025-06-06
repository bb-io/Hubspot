﻿using Apps.Hubspot.Constants;
using Apps.Hubspot.Services.ContentServices;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Blackbird.Applications.Sdk.Common.Exceptions;
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
    }      public IContentService GetContentService(string contentType)
    {
        return contentType switch
        {
            ContentTypes.Blog => new BlogPostService(invocationContext),
            ContentTypes.BlogAuthor => new BlogAuthorService(invocationContext),
            ContentTypes.BlogTag => new BlogTagService(invocationContext),
            ContentTypes.BlogComment => new BlogCommentService(invocationContext),
            ContentTypes.LandingPage => new LandingPageService(invocationContext),
            ContentTypes.Email => new MarketingEmailService(invocationContext),
            ContentTypes.Form => new MarketingFormService(invocationContext),
            ContentTypes.SitePage => new SitePageService(invocationContext),
            _ => throw new PluginApplicationException($"Unexpected content type received: {contentType}")
        };
    }
}