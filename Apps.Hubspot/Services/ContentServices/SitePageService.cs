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
            Type = ContentTypes.Form,
            CreatedAt = StringToDateTimeConverter.ToDateTime(x.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(x.Updated)
        }).ToList();
    }
    
    public override async Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);
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

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<GenericPageDto>(request);
        
        var htmlBytes =
            HtmlConverter.ToHtml(page.LayoutSections, page.HtmlTitle, page.Language, id, ContentTypes.SitePage, null);
        return new MemoryStream(htmlBytes);
    }

    public override async Task UpdateContentFromHtmlAsync(string targetLanguage, Stream stream)
    {
        var (pageInfo, json) = HtmlConverter.ToJson(stream);

        var sourcePageId = pageInfo.HtmlDocument.ExtractBlackbirdReferenceId() ?? throw new Exception("The source page ID is missing. Provide it as an optional input or add it to the HTML file");
        var content = await GetContentAsync(sourcePageId);
        var primaryLanguage = string.IsNullOrEmpty(pageInfo.Language) ? content.Language : pageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage))
        {
            throw new PluginMisconfigurationException("You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        }
        
        var translationId = await GetOrCreateTranslationId(ApiEndpoints.SitePages, sourcePageId, targetLanguage, primaryLanguage);
        await UpdateTranslatedPage(ApiEndpoints.UpdatePage(translationId), new()
        {
            Id = translationId,
            HtmlTitle = pageInfo.Title,
            LayoutSections = json
        });
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