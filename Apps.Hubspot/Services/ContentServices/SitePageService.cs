using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class SitePageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query)
    {
        var endpoint = ApiEndpoints.SitePages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        return response.Select(x => new Metadata
        {
            Id = x.Id,
            Title = x.Name,
            Domain = x.Domain,
            Language = x.Language!,
            State = x.CurrentState,
            Published = x.Published,
            Type = ContentTypes.SitePage,
            CreatedAt = StringToDateTimeConverter.ToDateTime(x.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(x.Updated)
        }).ToList();
    }
    
    public override async Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageWithTranslationsDto>(request);
        return await GetTranslatedLocalesResponse(page.Language ?? string.Empty, page.Translations);    
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);
        
        return new()
        {
            Id = page.Id,
            Title = page.Name,
            Domain = page.Domain,
            Language = page.Language!,
            State = page.CurrentState,
            Published = page.Published,
            Type = ContentTypes.SitePage,
            CreatedAt = StringToDateTimeConverter.ToDateTime(page.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(page.Updated)
        };
    }

    public async Task<PageWithTranslationsDto> GetPageAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<PageWithTranslationsDto>(request);
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<GenericPageDto>(request);
        
        var htmlBytes =
            HtmlConverter.ToHtml(page.LayoutSections, page.HtmlTitle, page.Language, id, ContentTypes.SitePage, null);
        return new MemoryStream(htmlBytes);
    }

    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var resultEntity = await HtmlConverter.ToJsonAsync(targetLanguage, stream, uploadContentRequest, InvocationContext);
        string sourcePageId = await GetSourcePageId(uploadContentRequest, resultEntity.PageInfo.HtmlDocument);
        var content = await GetContentAsync(sourcePageId);
        var primaryLanguage = string.IsNullOrEmpty(resultEntity.PageInfo.Language) ? content.Language : resultEntity.PageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage))
        {
            throw new PluginMisconfigurationException("You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        }

        var translationId = await GetOrCreateTranslationId(ApiEndpoints.SitePages, sourcePageId, targetLanguage, primaryLanguage);
        var pageDto = await UpdateTranslatedPage<PageDto>(ApiEndpoints.UpdatePage(translationId), new()
        {
            Id = translationId,
            HtmlTitle = resultEntity.PageInfo.Title,
            LayoutSections = resultEntity.Json
        });

        return new()
        {
            Id = pageDto.Id,
            Title = pageDto.Name,
            Domain = pageDto.Domain,
            Language = pageDto.Language!,
            State = pageDto.CurrentState,
            Published = pageDto.Published,
            Type = ContentTypes.SitePage,
            CreatedAt = StringToDateTimeConverter.ToDateTime(pageDto.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(pageDto.Updated)
        };
    }

    private async Task<string> GetSourcePageId(UploadContentRequest uploadContentRequest, HtmlDocument htmlDocument)
    {
        string sourcePageId;
        if (!string.IsNullOrEmpty(uploadContentRequest.SitePageId))
        {
            sourcePageId = uploadContentRequest.SitePageId;
        }
        else
        {
            var currentPageId = htmlDocument.ExtractBlackbirdReferenceId();

            var url = ApiEndpoints.ASitePage(currentPageId);
            var request = new HubspotRequest(url, Method.Get, Creds);
            var page = await Client.ExecuteWithErrorHandling<PageDto>(request);

            if (!string.IsNullOrEmpty(page.TranslatedFromId))
            {
                sourcePageId = page.TranslatedFromId;
            }
            else
            {
                sourcePageId = currentPageId;
            }
        }

        return sourcePageId;
    }

    public override async Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {        
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = updateContentRequest.Title,
            });
        
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);
        return new()
        {
            Id = page.Id,
            Title = page.Name,
            Domain = page.Domain,
            Language = page.Language!,
            State = page.CurrentState,
            Published = page.Published,
            Type = ContentTypes.SitePage,
            CreatedAt = StringToDateTimeConverter.ToDateTime(page.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(page.Updated)
        };
    }

    public override Task DeleteContentAsync(string id)
    {        
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }
}