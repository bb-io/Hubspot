using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Dtos.Emails;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Emails;
using Apps.Hubspot.Models.Requests.Forms;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Models.Responses.Emails;
using Apps.Hubspot.Models.Responses.Forms;
using Apps.Hubspot.Models.Responses.Pages;
using Apps.Hubspot.Utils.Converters;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Hubspot.Webhooks;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : HubSpotInvocable(invocationContext)
{
    [PollingEvent("On content created or updated",
        Description =
            "Triggered at specified time intervals and returns all blog posts, landing pages, site pages, emails, and forms that were updated or created during the specified time interval")]
    public async Task<PollingEventResponse<PageMemory, SearchMetadataResponse>> OnContentCreatedOrUpdated(PollingEventRequest<PageMemory> request, 
        [PollingEventParameter] LanguageRequest languageRequest)
    {
        if(request.Memory is null || request.Memory.LastPollingTime is null)
        {
            return new PollingEventResponse<PageMemory, SearchMetadataResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        var metadata = new List<Metadata>();
        
        var blogPosts = await OnBlogPostsCreatedOrUpdated(request, languageRequest);
        if (blogPosts.FlyBird && blogPosts.Result != null)
        {
            metadata.AddRange(blogPosts.Result?.BlogPosts.Select(x => new Metadata
            {
                Id = x.Id,
                Language = x.Language,
                Title = x.Name,
                CreatedAt = StringToDateTimeConverter.ToDateTime(x.Created),
                UpdatedAt = StringToDateTimeConverter.ToDateTime(x.Updated),
                Type = ContentTypes.Blog
            }));
        }
        
        var sitePages = await OnSitePageCreatedOrUpdated(request, languageRequest);
        if (sitePages.FlyBird && sitePages.Result != null)
        {
            metadata.AddRange(sitePages.Result.Pages.Select(x => new Metadata
            {
                Id = x.Id,
                Language = x.Language,
                Title = x.Name,
                CreatedAt = StringToDateTimeConverter.ToDateTime(x.Created),
                UpdatedAt = StringToDateTimeConverter.ToDateTime(x.Updated),
                Type = ContentTypes.SitePage
            }));
        }
        
        var landingPages = await OnLandingPageCreatedOrUpdated(request, languageRequest);
        if (landingPages.FlyBird && landingPages.Result != null)
        {
            metadata.AddRange(landingPages.Result.Pages.Select(x => new Metadata()
            {
                Id = x.Id,
                Language = x.Language,
                Title = x.Name,
                CreatedAt = StringToDateTimeConverter.ToDateTime(x.Created),
                UpdatedAt = StringToDateTimeConverter.ToDateTime(x.Updated),
                Type = ContentTypes.LandingPage
            }));
        }
        
        var marketingForms = await OnMarketingFormsCreatedOrUpdated(request, languageRequest);
        if (marketingForms.FlyBird && marketingForms.Result != null)
        {
            metadata.AddRange(marketingForms.Result.Forms.Select(x => new Metadata()
            {
                Id = x.Id,
                Language = x.Configuration.Language,
                Title = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Type = ContentTypes.Form
            }));
        }
        
        var marketingEmails = await OnMarketingEmailsCreatedOrUpdated(request, languageRequest);
        if (marketingEmails.FlyBird && marketingEmails.Result != null)
        {
            metadata.AddRange(marketingEmails.Result.Emails.Select(x => new Metadata()
            {
                Id = x.Id,
                Language = x.Language,
                Title = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt ?? DateTime.MinValue,
                Type = ContentTypes.Email
            }));
        }

        return new()
        {
            FlyBird = metadata.Any(),
            Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
            Result = new(metadata)
        };
    }
    
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
        var blogPosts = createdBlogPosts.Items
            .Concat(updatedBlogPosts.Items)
            .Where(p => p.Language == languageRequest.Language)
            .DistinctBy(x => x.Id)
            .ToList();
        
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
        
        var created = await GetAllSitePages(new SearchPagesRequest { CreatedAfter = request.Memory.LastPollingTime.Value });
        var updated = await GetAllSitePages(new SearchPagesRequest { UpdatedAfter = request.Memory.LastPollingTime.Value });
        var pages = created.Items
            .Concat(updated.Items)
            .Where(p => p.Language == languageRequest.Language)
            .DistinctBy(x => x.Id)
            .ToList();
        
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
        var pages = created.Items
            .Concat(updated.Items)
            .Where(p => p.Language == languageRequest.Language)
            .DistinctBy(x => x.Id)
            .ToList();
        
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
    
    [PollingEvent("On marketing forms created or updated",
        Description = "Triggered when a marketing forms is created or updated")]
    public async Task<PollingEventResponse<PageMemory, MarketingFormsResponse>>
        OnMarketingFormsCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        if(request.Memory is null || request.Memory.LastPollingTime is null)
        {
            return new PollingEventResponse<PageMemory, MarketingFormsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        var created = await GetAllMarketingForms(new() { CreatedAfter = request.Memory.LastPollingTime.Value });
        var updated = await GetAllMarketingForms(new () { UpdatedAfter = request.Memory.LastPollingTime.Value });
        var forms = created.Items
            .Concat(updated.Items)
            .Where(p => p.Configuration.Language == languageRequest.Language)
            .DistinctBy(x => x.Id)
            .ToList();
        
        if(forms.Count == 0)
        {
            return new PollingEventResponse<PageMemory, MarketingFormsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        return new PollingEventResponse<PageMemory, MarketingFormsResponse>
        {
            FlyBird = true,
            Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
            Result = new(forms)
        };
    }

    [PollingEvent("On marketing emails created or updated",
        Description = "Triggered when a marketing emails is created or updated")]
    public async Task<PollingEventResponse<PageMemory, MarketingEmailsResponse>>
        OnMarketingEmailsCreatedOrUpdated(PollingEventRequest<PageMemory> request,
            [PollingEventParameter] LanguageRequest languageRequest)
    {
        if(request.Memory is null || request.Memory.LastPollingTime is null)
        {
            return new PollingEventResponse<PageMemory, MarketingEmailsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        var created = await GetAllMarketingEmails(new() { CreatedAfter = request.Memory.LastPollingTime.Value });
        var updated = await GetAllMarketingEmails(new () { UpdatedAfter = request.Memory.LastPollingTime.Value });
        var emails = created.Items
            .Concat(updated.Items)
            .Where(p => p.Language == languageRequest.Language)
            .DistinctBy(x => x.Id)
            .ToList();
        
        if(emails.Count == 0)
        {
            return new PollingEventResponse<PageMemory, MarketingEmailsResponse>
            {
                FlyBird = false,
                Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
                Result = null
            };
        }
        
        return new PollingEventResponse<PageMemory, MarketingEmailsResponse>
        {
            FlyBird = true,
            Memory = new PageMemory { LastPollingTime = DateTime.UtcNow },
            Result = new(emails)
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
    
    public async Task<ListResponse<MarketingFormDto>> GetAllMarketingForms(TimeFilterRequest input)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var result = await Client.Paginate<MarketingFormDto>(request);

        if (input.CreatedAfter.HasValue)
        {
            result = result.Where(x => x.CreatedAt > input.CreatedAfter.Value).ToList();
        }
        
        if (input.UpdatedAfter.HasValue)
        {
            result = result.Where(x => x.UpdatedAt > input.UpdatedAfter.Value).ToList();
        }
        
        return new(result);
    }
    
    public async Task<ListResponse<MarketingEmailDto>> GetAllMarketingEmails(SearchEmailsRequest input)
    {
        var query = input.AsQuery();
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}".WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var result = await Client.Paginate<MarketingEmailDto>(request);
        return new(result);
    }
}