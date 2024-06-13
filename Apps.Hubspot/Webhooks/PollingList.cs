using Apps.Hubspot.Actions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses.Pages;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

namespace Apps.Hubspot.Webhooks;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : HubSpotInvocable(invocationContext)
{
    [PollingEvent("On blog posts created or updated", Description = "Triggered when a blog posts is created or updated")]
    public async Task<PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>>
        OnBlogPostsCreatedOrUpdated(PollingEventRequest<BlogPostsCreatedOrUpdatedMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        var blogPostActions = new BlogPostsActions(InvocationContext, null);
        var blogPosts = await blogPostActions.GetAllBlogPosts(new SearchPagesRequest());
        var pages = blogPosts.Items.ToList();
            
        if (request.Memory == null)
        {
            return await HandleFirstRunAsync(pages);
        }

        return await HandleSubsequentRunsAsync(request, languageRequest, pages);
    }
    
    [PollingEvent("On site pages created or updated",
        Description = "Triggered when a site pages is created or updated")]
    public async Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        OnSitePageCreatedOrUpdated(PollingEventRequest<PageCreatedOrUpdatedMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        var pageActions = new PageActions(InvocationContext, null);
        var sitePages = await pageActions.GetAllSitePages(new SearchPagesRequest());
        
        var pages = sitePages.Items.ToList();
        if (request.Memory == null)
        {
            return await HandleFirstRunAsync(pages);
        }

        return await HandleSubsequentRunsAsync(request, languageRequest, pages);
    }

    [PollingEvent("On landing pages created or updated",
        Description = "Triggered when a landing pages is created or updated")]
    public async Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        OnLandingPageCreatedOrUpdated(PollingEventRequest<PageCreatedOrUpdatedMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        var landingPageActions = new LandingPageActions(InvocationContext, null);
        var landingPages = await landingPageActions.GetAllLandingPages(new SearchPagesRequest());
        
        var pages = landingPages.Items.ToList();
        if (request.Memory == null)
        {
            return await HandleFirstRunAsync(pages);
        }

        return await HandleSubsequentRunsAsync(request, languageRequest, pages);
    }

    private Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        HandleFirstRunAsync(List<PageDto> pages)
    {
        return Task.FromResult(new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
        {
            FlyBird = false,
            Memory = new PageCreatedOrUpdatedMemory() { Pages = pages },
            Result = null
        });
    }

    private Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        HandleSubsequentRunsAsync(PollingEventRequest<PageCreatedOrUpdatedMemory> request,
            LanguageRequest languageRequest, 
            List<PageDto> pages)
    {
        if (pages.Count == 0)
        {
            return Task.FromResult(new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            });
        }

        var newPages = pages.Where(p => request.Memory.Pages.All(mp => mp.Id != p.Id)).ToList();
        var updatedPages = pages
            .Where(p => request.Memory.Pages.Any(mp => mp.Id == p.Id && mp.HasSignificantChanges(p))).ToList();

        var allChanges = newPages.Concat(updatedPages).ToList();
        if (allChanges.Count == 0)
        {
            return Task.FromResult(new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            });
        }
        
        if(languageRequest.Language != null)
        {
            allChanges = allChanges.Where(p => p.Language == languageRequest.Language).ToList();
        }

        return Task.FromResult(new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
        {
            FlyBird = true,
            Memory = new PageCreatedOrUpdatedMemory() { Pages = pages },
            Result = new PagesResponse() { Pages = allChanges }
        });
    }
    
    
    private Task<PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>> HandleFirstRunAsync(
        List<BlogPostDto> pages)
    {
        return Task.FromResult(new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
        {
            FlyBird = false,
            Memory = new BlogPostsCreatedOrUpdatedMemory() { BlogPosts = pages },
            Result = null
        });
    }

    private Task<PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>>
        HandleSubsequentRunsAsync(PollingEventRequest<BlogPostsCreatedOrUpdatedMemory> request, 
            LanguageRequest languageRequest, 
            List<BlogPostDto> pages)
    {
        if (pages.Count == 0)
        {
            return Task.FromResult(new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            });
        }

        var newPages = pages.Where(p => request.Memory.BlogPosts.All(mp => mp.Id != p.Id)).ToList();
        var updatedPages = pages
            .Where(p => request.Memory.BlogPosts.Any(mp => mp.Id == p.Id && mp.HasSignificantChanges(p))).ToList();

        var allChanges = newPages.Concat(updatedPages).ToList();
        if (allChanges.Count == 0)
        {
            return Task.FromResult(new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            });
        }
        
        if(languageRequest.Language != null)
        {
            allChanges = allChanges.Where(p => p.Language == languageRequest.Language).ToList();
        }

        return Task.FromResult(new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
        {
            FlyBird = true,
            Memory = new BlogPostsCreatedOrUpdatedMemory() { BlogPosts = pages },
            Result = new BlogPostsResponse() { BlogPosts = allChanges }
        });
    }
}