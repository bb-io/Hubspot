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

public class LandingPageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest)
    {
        var endpoint = ApiEndpoints.LandingPages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);
        if (!string.IsNullOrEmpty(searchContentRequest.UrlContains))
        {
            response = response.Where(x => x.Url.Contains(searchContentRequest.UrlContains, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        var activityInfo = await GetActivityInfo();
        return response.Select(x => ConvertLandingPageToMetadata(x, activityInfo)).ToList();
    }

    public async Task<PageWithTranslationsDto> GetLandingPageAsync(string id)
    {
        var url = ApiEndpoints.ALandingPage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<PageWithTranslationsDto>(request);
    }

    public override async Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        var url = ApiEndpoints.ALandingPage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageWithTranslationsDto>(request);
        return await GetTranslatedLocalesResponse(page.Language ?? string.Empty, page.Translations);
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var url = ApiEndpoints.ALandingPage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);
        var activityInfo = await GetActivityInfo();
        return ConvertLandingPageToMetadata(page, activityInfo);
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var url = ApiEndpoints.ALandingPage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var result = await Client.ExecuteWithErrorHandling<GenericPageDto>(request);
        var activityInfo = await GetActivityInfo();

        var htmlFile = HtmlConverter.ToHtml(result.LayoutSections, result.HtmlTitle, result.Language!, id, result.TranslatedFromId, ContentTypes.LandingPage, null, result.Slug, result.Url, $"https://app.hubspot.com/pages/{activityInfo.PortalId}/editor/{id}/content", result.MetaDescription, string.Empty);
        return new MemoryStream(htmlFile);
    }

    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var resultEntity = await HtmlConverter.ToJsonAsync(targetLanguage, stream, uploadContentRequest, InvocationContext);

        var sourcePageId = resultEntity.PageInfo.HtmlDocument.ExtractBlackbirdReferenceId() ?? throw new Exception("The Source page ID is missing");
        var content = await GetContentAsync(sourcePageId);
        var primaryLanguage = string.IsNullOrEmpty(resultEntity.PageInfo.Language) ? content.Language : resultEntity.PageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage))
        {
            throw new PluginMisconfigurationException(
                "You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        }

        var translationId = await GetOrCreateTranslationId(ApiEndpoints.LandingPages, sourcePageId, targetLanguage, primaryLanguage);
        var pageDto = await UpdateTranslatedPage<PageDto>(ApiEndpoints.UpdateLandingPage(translationId), new()
        {
            Id = translationId,
            HtmlTitle = resultEntity.PageInfo.Title,
            LayoutSections = resultEntity.Json
        });
        var activityInfo = await GetActivityInfo();
        return ConvertLandingPageToMetadata(pageDto, activityInfo);
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
        var activityInfo = await GetActivityInfo();
        return ConvertLandingPageToMetadata(page, activityInfo);
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.LandingPages}/{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    private Metadata ConvertLandingPageToMetadata(PageDto page, ActivityInfo activityInfo)
    {
        return new Metadata
        {
            ContentId = page.Id,
            Title = page.Name,
            Domain = page.Domain,
            Language = page.Language!,
            State = page.CurrentState,
            Published = page.Published,
            Type = ContentTypes.LandingPage,
            Url = page.Url,
            AdminUrl = $"https://app.hubspot.com/pages/{activityInfo.PortalId}/editor/{page.Id}/content",
            Slug = page.Slug,
            CreatedAt = StringToDateTimeConverter.ToDateTime(page.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(page.Updated),
            TranslatedFromId = page.TranslatedFromId,
            UpdatedByUserId = page.UpdatedById
        };
    }
}