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
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Microsoft.Extensions.Logging;
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
        try
        {
            await Logger.LogAsync(new
            {
                message = "On blog posts created or updated"
            });
            
            var blogPosts = await GetAllBlogPosts(new SearchPagesRequest());
            var pages = blogPosts.Items.ToList();

            return HandleBlogPostPollingEventAsync(request, languageRequest, pages);
        }
        catch (Exception e)
        {
            await Logger.LogExceptionAsync(e);
            throw;
        }
    }

    [PollingEvent("On site pages created or updated", Description = "Triggered when a site page is created or updated")]
    public async Task<PollingEventResponse<PageMemory, PagesResponse>>
        OnSitePageCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        try
        {
            var sitePages = await GetAllSitePages(new SearchPagesRequest());
            var pages = sitePages.Items.ToList();

            return HandlePagePollingEventAsync(request, languageRequest, pages);
        }
        catch (Exception e)
        {
            await Logger.LogExceptionAsync(e);
            throw;
        }
    }

    [PollingEvent("On landing pages created or updated", Description = "Triggered when a landing page is created or updated")]
    public async Task<PollingEventResponse<PageMemory, PagesResponse>>
        OnLandingPageCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        try
        {
            var landingPages = await GetAllLandingPages(new SearchPagesRequest());
            var pages = landingPages.Items.ToList();

            return HandlePagePollingEventAsync(request, languageRequest, pages);
        }
        catch (Exception e)
        {
            await Logger.LogExceptionAsync(e);
            throw;
        }
    }

    private PollingEventResponse<PageMemory, BlogPostsResponse> HandleBlogPostPollingEventAsync(
        PollingEventRequest<PageMemory> request,
        LanguageRequest languageRequest,
        List<BlogPostDto> blogPosts)
    {
        if (request.Memory is null)
        {
            Logger.Log(new
            {
                message = "First run of the polling event",
                memory = request.Memory,
                blog_posts = blogPosts
            });
            
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { Pages = blogPosts.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created.ToString(CultureInfo.InvariantCulture),
                    Updated = p.Updated.ToString(CultureInfo.InvariantCulture)
                }).ToList() },
                Result = null
            };
        }
        
        if (blogPosts.Count == 0)
        {
            Logger.Log(new
            {
                message = "No blog posts found",
                memory = request.Memory
            });
            
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = request.Memory,
                Result = null
            };
        }

        var memoryEntities = request.Memory.Pages;
        var newPages = blogPosts.Where(x => memoryEntities.All(y => x.Id != y.Id)).ToList();
        var updatedPages = blogPosts
            .Where(p => memoryEntities.Any(mp => mp.Id == p.Id && DateTime.Parse(mp.Updated, null, DateTimeStyles.RoundtripKind) < DateTime.Parse(p.Updated, null, DateTimeStyles.RoundtripKind)))
            .ToList();

        var allChanges = newPages.Concat(updatedPages).ToList();
        if (allChanges.Count == 0)
        {
            Logger.Log(new
            {
                message = "No changes found",
                memory = new PageMemory { Pages = blogPosts.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created.ToString(CultureInfo.InvariantCulture),
                    Updated = p.Updated.ToString(CultureInfo.InvariantCulture)
                }).ToList() }
            });
            
            return new PollingEventResponse<PageMemory, BlogPostsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { Pages = blogPosts.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created,
                    Updated = p.Updated
                }).ToList() },
                Result = null
            };
        }

        allChanges = allChanges.Where(p => p.Language == languageRequest.Language).ToList();
        
        Logger.Log(new
        {
            new_pages = newPages,
            updated_pages = updatedPages,
            all_changes = allChanges
        });

        return new PollingEventResponse<PageMemory, BlogPostsResponse>
        {
            FlyBird = true,
            Memory = new PageMemory
            {
                Pages = blogPosts.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created,
                    Updated = p.Updated
                }).ToList()
            },
            Result = new BlogPostsResponse
            {
                BlogPosts = allChanges
            }
        };
    }

    private PollingEventResponse<PageMemory, PagesResponse> HandlePagePollingEventAsync(
        PollingEventRequest<PageMemory> request,
        LanguageRequest languageRequest,
        List<PageDto> pages)
    {
        if (request.Memory is null)
        {
            Logger.Log(new
            {
                message = "First run of the polling event",
                memory = request.Memory,
                pages = pages
            });
            
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { Pages = pages.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created.ToString(CultureInfo.InvariantCulture),
                    Updated = p.Updated.ToString(CultureInfo.InvariantCulture)
                }).ToList() },
                Result = null
            };
        }
        
        if (pages.Count == 0)
        {
            Logger.Log(new
            {
                message = "No pages found",
                memory = request.Memory
            });
            
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
            .Where(p => memoryEntities.Any(mp => mp.Id == p.Id && DateTime.Parse(mp.Updated) < p.Updated))
            .ToList();

        var allChanges = newPages.Concat(updatedPages).ToList();
        if (allChanges.Count == 0)
        {
            Logger.Log(new
            {
                message = "No changes found",
                memory = new PageMemory { Pages = pages.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created.ToString(CultureInfo.InvariantCulture),
                    Updated = p.Updated.ToString(CultureInfo.InvariantCulture)
                }).ToList() }
            });
            
            return new PollingEventResponse<PageMemory, PagesResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { Pages = pages.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created.ToString(CultureInfo.InvariantCulture),
                    Updated = p.Updated.ToString(CultureInfo.InvariantCulture)
                }).ToList() },
                Result = null
            };
        }

        allChanges = allChanges.Where(p => p.Language == languageRequest.Language).ToList();
        
        Logger.Log(new
        {
            new_pages = newPages,
            updated_pages = updatedPages,
            all_changes = allChanges
        });

        return new PollingEventResponse<PageMemory, PagesResponse>
        {
            FlyBird = true,
            Memory = new PageMemory
            {
                Pages = pages.Select(p => new PageEntity
                {
                    Id = p.Id,
                    Created = p.Created.ToString(CultureInfo.InvariantCulture),
                    Updated = p.Updated.ToString(CultureInfo.InvariantCulture)
                }).ToList()
            },
            Result = new PagesResponse
            {
                Pages = allChanges
            }
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
