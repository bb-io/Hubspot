using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class LandingPageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query)
    {
        var endpoint = ApiEndpoints.LandingPages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        return response.Select(x => new Metadata
        {
            Id = x.Id,
            Title = x.Name,
            Language = x.Language!,
            State = x.CurrentState,
            Published = x.Published,
            Type = ContentTypes.LandingPage,
            CreatedAt = StringToDateTimeConverter.ToDateTime(x.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(x.Updated)
        }).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var url = ApiEndpoints.ALandingPage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);

        return new()
        {
            Id = page.Id,
            Title = page.Name,
            State = page.CurrentState,
            Published = page.Published,
            Language = page.Language!,
            Type = ContentTypes.LandingPage,
            CreatedAt = StringToDateTimeConverter.ToDateTime(page.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(page.Updated)
        };
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var url = ApiEndpoints.ALandingPage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var result = await Client.ExecuteWithErrorHandling<GenericPageDto>(request);
        
        var htmlFile = HtmlConverter.ToHtml(result.LayoutSections, result.HtmlTitle, result.Language!, id, ContentTypes.LandingPage, null);
        return new MemoryStream(htmlFile);
    }

    public override async Task UpdateContentFromHtmlAsync(string targetLanguage, Stream stream)
    {
        var (pageInfo, json) = HtmlConverter.ToJson(stream);

        var sourcePageId = pageInfo.HtmlDocument.ExtractBlackbirdReferenceId() ?? throw new Exception("The Source page ID is missing");
        var content = await GetContentAsync(sourcePageId);
        var primaryLanguage = string.IsNullOrEmpty(pageInfo.Language) ? content.Language : pageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage))
        {
            throw new PluginMisconfigurationException(
                "You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        }
        
        var translationId = await GetOrCreateTranslationId(ApiEndpoints.LandingPages, sourcePageId, targetLanguage, primaryLanguage);
        await UpdateTranslatedPage(ApiEndpoints.UpdateLandingPage(translationId), new()
        {
            Id = translationId,
            HtmlTitle = pageInfo.Title,
            LayoutSections = json
        });
    }

    public override async Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {
        var url = ApiEndpoints.ALandingPage(id);
        var request = new HubspotRequest(url, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = updateContentRequest.Title
            });
        
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);
        return new()
        {
            Id = page.Id,
            Title = page.Name,
            Language = page.Language!,
            State = page.CurrentState,
            Published = page.Published,
            Type = ContentTypes.LandingPage,
            CreatedAt = StringToDateTimeConverter.ToDateTime(page.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(page.Updated)
        };
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.LandingPages}/{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }
}