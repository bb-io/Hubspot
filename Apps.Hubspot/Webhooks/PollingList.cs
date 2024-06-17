using System.Globalization;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Pages;
using Apps.Hubspot.Utils;
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
        var blogPosts = await GetAllBlogPosts(new SearchPagesRequest());
        var pages = blogPosts.Items.ToList();
        return HandleBlogPostPollingEventAsync(request, languageRequest, pages);
    }

    [PollingEvent("On site pages created or updated", Description = "Triggered when a site page is created or updated")]
    public async Task<PollingEventResponse<PageMemory, PagesResponse>>
        OnSitePageCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        var sitePages = await GetAllSitePages(new SearchPagesRequest());
        var pages = sitePages.Items.ToList();
        return HandlePagePollingEventAsync(request, languageRequest, pages);
    }

    [PollingEvent("On landing pages created or updated",
        Description = "Triggered when a landing page is created or updated")]
    public async Task<PollingEventResponse<PageMemory, PagesResponse>>
        OnLandingPageCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        var landingPages = await GetAllLandingPages(new SearchPagesRequest());
        var pages = landingPages.Items.ToList();
        return HandlePagePollingEventAsync(request, languageRequest, pages);
    }

    private PollingEventResponse<PageMemory, BlogPostsResponse> HandleBlogPostPollingEventAsync(
        PollingEventRequest<PageMemory> request,
        LanguageRequest languageRequest,
        List<BlogPostDto> blogPosts)
    {
        if (request.Memory is null)
        {
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory(blogPosts),
                Result = null
            };
        }

        if (blogPosts.Count == 0)
        {
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        var memoryEntities = request.Memory.Pages;
        var newPages = blogPosts.Where(p => memoryEntities.All(mp => mp.Id != p.Id)).ToList();
        var updatedPages = blogPosts
            .Where(p => DateTimeHelper.IsPageUpdated(memoryEntities, new PageEntity(p.Id, p.Created, p.Updated)))
            .ToList();

        var allChanges = newPages.Concat(updatedPages)
            .Where(p => p.Language == languageRequest.Language)
            .ToList();
        if (allChanges.Count == 0)
        {
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory(blogPosts),
                Result = null
            };
        }

        return new PollingEventResponse<PageMemory, BlogPostsResponse>
        {
            FlyBird = true,
            Memory = new PageMemory(blogPosts),
            Result = new BlogPostsResponse(allChanges)
        };
    }

    private PollingEventResponse<PageMemory, PagesResponse> HandlePagePollingEventAsync(
        PollingEventRequest<PageMemory> request,
        LanguageRequest languageRequest,
        List<PageDto> pages)
    {
        if (request.Memory is null)
        {
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory(pages),
                Result = null
            };
        }

        if (pages.Count == 0)
        {
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        var memoryEntities = request.Memory.Pages;
        var newPages = pages.Where(p => memoryEntities.All(mp => mp.Id != p.Id)).ToList();
        var updatedPages = pages
            .Where(p => DateTimeHelper.IsPageUpdated(memoryEntities, new PageEntity(p.Id, p.Created, p.Updated)))
            .ToList();
        
        Logger.Log(new
        {
            new_pages = newPages,
            updated_pages = updatedPages,
            memory_entities = memoryEntities,
            pages
        });

        var allChanges = newPages.Concat(updatedPages)
            .Where(p => p.Language == languageRequest.Language)
            .ToList();
        
        if (allChanges.Count == 0)
        {
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory(pages),
                Result = null
            };
        }

        return new PollingEventResponse<PageMemory, PagesResponse>
        {
            FlyBird = true,
            Memory = new PageMemory(pages),
            Result = new PagesResponse(allChanges)
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