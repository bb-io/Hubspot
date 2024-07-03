using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Pages;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Hubspot.Webhooks;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : HubSpotInvocable(invocationContext)
{
    [PollingEvent("On blog posts created or updated", Description = "Triggered when a blog post is created or updated")]
    public async Task<PollingEventResponse<PageMemory, BlogPostsResponse>>
        OnBlogPostsCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        if(request.Memory is null || request.Memory.LastPollingTime is null)
        {
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        var createdBlogPosts = await GetAllBlogPosts(new SearchPagesRequest { CreatedAfter = request.Memory.LastPollingTime.Value });
        var updatedBlogPosts = await GetAllBlogPosts(new SearchPagesRequest { UpdatedAfter = request.Memory.LastPollingTime.Value });
        var blogPosts = createdBlogPosts.Items.Concat(updatedBlogPosts.Items).ToList();
        if(blogPosts.Count == 0)
        {
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        return new PollingEventResponse<PageMemory, BlogPostsResponse>
        {
            FlyBird = true,
            Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
            Result = new BlogPostsResponse(blogPosts)
        };
    }

    [PollingEvent("On site pages created or updated", Description = "Triggered when a site page is created or updated")]
    public async Task<PollingEventResponse<PageMemory, PagesResponse>>
        OnSitePageCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        if(request.Memory is null || request.Memory.LastPollingTime is null)
        {
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        var created = await GetAllSitePages(new SearchPagesRequest() { CreatedAfter = request.Memory.LastPollingTime.Value });
        var updated = await GetAllSitePages(new SearchPagesRequest() { UpdatedAfter = request.Memory.LastPollingTime.Value });
        var pages = created.Items.Concat(updated.Items).ToList();
        if(pages.Count == 0)
        {
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        return new PollingEventResponse<PageMemory, PagesResponse>
        {
            FlyBird = true,
            Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
            Result = new PagesResponse(pages)
        };
    }

    [PollingEvent("On landing pages created or updated",
        Description = "Triggered when a landing page is created or updated")]
    public async Task<PollingEventResponse<PageMemory, PagesResponse>>
        OnLandingPageCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        if(request.Memory is null || request.Memory.LastPollingTime is null)
        {
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        var created = await GetAllLandingPages(new SearchPagesRequest { CreatedAfter = request.Memory.LastPollingTime.Value });
        var updated = await GetAllLandingPages(new SearchPagesRequest { UpdatedAfter = request.Memory.LastPollingTime.Value });
        var pages = created.Items.Concat(updated.Items).ToList();
        if(pages.Count == 0)
        {
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        return new PollingEventResponse<PageMemory, PagesResponse>
        {
            FlyBird = true,
            Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
            Result = new PagesResponse(pages)
        };
    }

    private async Task<ListResponse<BlogPostDto>> GetAllBlogPosts(SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.BlogPostsSegment.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<BlogPostWithTranslationsDto>(request);

        if (input.NotTranslatedInLanguage != null)
        {
            response = response.Where(p =>
                p.Translations == null ||
                p.Translations.Keys.All(key => key != input.NotTranslatedInLanguage.ToLower())).ToList();
        }

        return new(response);
    }

    private async Task<ListResponse<PageDto>> GetAllSitePages(SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.SitePages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        if (input.NotTranslatedInLanguage != null)
        {
            response = response.Where(p =>
                p.Translations == null ||
                p.Translations.Keys.All(key => key != input.NotTranslatedInLanguage.ToLower())).ToList();
        }

        return new(response);
    }

    private async Task<ListResponse<PageDto>> GetAllLandingPages(SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.LandingPages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        if (input.NotTranslatedInLanguage != null)
        {
            response = response.Where(p =>
                p.Translations == null ||
                p.Translations.Keys.All(key => key != input.NotTranslatedInLanguage.ToLower())).ToList();
        }

        return new(response);
    }
}