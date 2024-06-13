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
        OnBlogPostsCreatedOrUpdated(PollingEventRequest<BlogPostsCreatedOrUpdatedMemory> request)
    {
        try
        {
            var blogPostActions = new BlogPostsActions(InvocationContext, null);
            var blogPosts = await blogPostActions.GetAllBlogPosts(new SearchPagesRequest());
            var pages = blogPosts.Items.ToList();

            await Logger.LogAsync(new { Request = request });

            if (request.Memory == null)
            {
                return await HandleFirstRunAsync(pages);
            }

            return await HandleSubsequentRunsAsync(request, pages);
        }
        catch (Exception e)
        {
            await Logger.LogAsync(e);

            return new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }
    }

    private async Task<PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>> HandleFirstRunAsync(
        List<BlogPostDto> pages)
    {
        await Logger.LogAsync(new
        {
            OrderedBlogPosts = pages,
            Message = "Last blog post if memory is null",
        });

        return new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
        {
            FlyBird = false,
            Memory = new BlogPostsCreatedOrUpdatedMemory() { BlogPosts = pages },
            Result = null
        };
    }

    private async Task<PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>>
        HandleSubsequentRunsAsync(PollingEventRequest<BlogPostsCreatedOrUpdatedMemory> request, List<BlogPostDto> pages)
    {
        await Logger.LogAsync(new
        {
            UpdatedBlogPosts = pages,
            Message = "Updated blog posts",
        });

        if (pages.Count == 0)
        {
            await Logger.LogAsync(new
            {
                Message = "No updated blog posts",
            });

            return new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        var newPages = pages.Where(p => request.Memory.BlogPosts.All(mp => mp.Id != p.Id)).ToList();
        var updatedPages = pages
            .Where(p => request.Memory.BlogPosts.Any(mp => mp.Id == p.Id && mp.HasSignificantChanges(p))).ToList();

        var allChanges = newPages.Concat(updatedPages).ToList();

        await Logger.LogAsync(new
        {
            NewPages = newPages,
            UpdatedPages = updatedPages,
            AllChanges = allChanges,
            Message = "Detected changes in blog posts",
        });

        if (allChanges.Count == 0)
        {
            await Logger.LogAsync(new
            {
                Message = "No new or updated blog posts",
            });

            return new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        return new PollingEventResponse<BlogPostsCreatedOrUpdatedMemory, BlogPostsResponse>()
        {
            FlyBird = true,
            Memory = new BlogPostsCreatedOrUpdatedMemory() { BlogPosts = pages },
            Result = new BlogPostsResponse() { BlogPosts = allChanges }
        };
    }


    [PollingEvent("On site pages created or updated",
        Description = "Triggered when a site pages is created or updated")]
    public async Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        OnSitePageCreatedOrUpdated(PollingEventRequest<PageCreatedOrUpdatedMemory> request)
    {
        var pageActions = new PageActions(InvocationContext, null);
        var sitePages = await pageActions.GetAllSitePages(new SearchPagesRequest());
        var pages = sitePages.Items.ToList();

        await Logger.LogAsync(new { Request = request });
        if (request.Memory == null)
        {
            return await HandleFirstRunAsync(pages);
        }

        return await HandleSubsequentRunsAsync(request, pages);
    }

    [PollingEvent("On landing pages created or updated",
        Description = "Triggered when a landing pages is created or updated")]
    public async Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        OnLandingPageCreatedOrUpdated(PollingEventRequest<PageCreatedOrUpdatedMemory> request)
    {
        var landingPageActions = new LandingPageActions(InvocationContext, null);
        var landingPages = await landingPageActions.GetAllLandingPages(new SearchPagesRequest());
        var pages = landingPages.Items.ToList();

        await Logger.LogAsync(new { Request = request });
        if (request.Memory == null)
        {
            return await HandleFirstRunAsync(pages);
        }

        return await HandleSubsequentRunsAsync(request, pages);
    }

    private async Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        HandleFirstRunAsync(List<PageDto> pages)
    {
        await Logger.LogAsync(new
        {
            OrderedLandingPages = pages,
            Message = "Last landing page if memory is null",
        });

        return new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
        {
            FlyBird = false,
            Memory = new PageCreatedOrUpdatedMemory() { Pages = pages },
            Result = null
        };
    }

    private async Task<PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>>
        HandleSubsequentRunsAsync(PollingEventRequest<PageCreatedOrUpdatedMemory> request, List<PageDto> pages)
    {
        await Logger.LogAsync(new
        {
            UpdatedLandingPages = pages,
            Message = "Updated landing pages",
        });

        if (pages.Count == 0)
        {
            await Logger.LogAsync(new
            {
                Message = "No updated landing pages",
            });

            return new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        var newPages = pages.Where(p => request.Memory.Pages.All(mp => mp.Id != p.Id)).ToList();
        var updatedPages = pages
            .Where(p => request.Memory.Pages.Any(mp => mp.Id == p.Id && mp.HasSignificantChanges(p))).ToList();

        var allChanges = newPages.Concat(updatedPages).ToList();

        await Logger.LogAsync(new
        {
            NewPages = newPages,
            UpdatedPages = updatedPages,
            AllChanges = allChanges,
            Message = "Detected changes in pages",
        });

        if (allChanges.Count == 0)
        {
            await Logger.LogAsync(new
            {
                Message = "No new or updated pages",
            });

            return new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        return new PollingEventResponse<PageCreatedOrUpdatedMemory, PagesResponse>()
        {
            FlyBird = true,
            Memory = new PageCreatedOrUpdatedMemory() { Pages = pages },
            Result = new PagesResponse() { Pages = allChanges }
        };
    }
}