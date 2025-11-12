using RestSharp;
using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Apps.Hubspot.Models.Requests;
using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests.LandingPages;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Apps.Hubspot.Models.Responses.Translations;
using Apps.Hubspot.Services;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Apps.Hubspot.Utils;

namespace Apps.Hubspot.Actions;

[ActionList("Landing page")]
public class LandingPageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BasePageActions(invocationContext, fileManagementClient)
{
    [Action("Search landing pages", Description = "Search for a list of site pages that match a certain criteria")]
    public async Task<ListResponse<PageDto>> GetAllLandingPages([ActionParameter] SearchPagesRequest searchPagesRequest)
    {
        if (searchPagesRequest.UpdatedByUserIdsWhitelist?.Any() == true && 
            searchPagesRequest.UpdatedByUserIdsBlacklist?.Any() == true)
        {
            throw new PluginMisconfigurationException("You cannot specify both whitelist and blacklist for updated by user IDs. Please use only one of them.");
        }

        var query = searchPagesRequest.AsHubspotQuery();
        if (searchPagesRequest.UpdatedByUserIdsWhitelist?.Count() == 1)
        {
            query.Add("updatedById__eq", searchPagesRequest.UpdatedByUserIdsWhitelist.First());
        }

        var endpoint = ApiEndpoints.LandingPages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        if (searchPagesRequest.NotTranslatedInLanguage != null)
        {
            response = response.Where(p => p.Translations == null || p.Translations.Keys.All(key => key != searchPagesRequest.NotTranslatedInLanguage.ToLower())).ToList();
        }

        if (!String.IsNullOrEmpty(searchPagesRequest.UrlContains))
        {
            response = response.Where(p => p.Url.Contains(searchPagesRequest.UrlContains)).ToList();
        }

        if (searchPagesRequest.UpdatedByUserIdsWhitelist?.Any() == true && searchPagesRequest.UpdatedByUserIdsWhitelist.Count() > 1)
        {
            response = response.Where(p => searchPagesRequest.UpdatedByUserIdsWhitelist.Contains(p.UpdatedById)).ToList();
        }

        if (searchPagesRequest.UpdatedByUserIdsBlacklist?.Any() == true)
        {
            response = response.Where(p => !searchPagesRequest.UpdatedByUserIdsBlacklist.Contains(p.UpdatedById)).ToList();
        }

        return new(response);
    }

    [Action("Get landing page", Description = "Get information of a specific landing page")]
    public Task<PageDto> GetLandingPage([ActionParameter] LandingPageRequest input)
    {
        PluginMisconfigurationExceptionHelper.ThrowIsNullOrEmpty(input.PageId, nameof(input.PageId));
        return GetPage<PageDto>(ApiEndpoints.ALandingPage(input.PageId));
    }
    
    [Action("Get landing page translation language codes", Description = "Returns list of translated locales for landing page based on ID")]
    public async Task<TranslatedLocalesResponse> GetListOfTranslatedLocales([ActionParameter] LandingPageRequest request)
    {
        ContentServicesFactory factory = new(invocationContext);
        var service = factory.GetContentService(ContentTypes.LandingPage);
        return await service.GetTranslationLanguageCodesAsync(request.PageId);
    }

    [Action("Get landing page as HTML file",
        Description = "Get information of a specific landing page and return an HTML file of its content")]
    public async Task<FileLanguageResponse> GetLandingPageAsHtml([ActionParameter] LandingPageRequest input, 
        [ActionParameter] LocalizablePropertiesRequest Properties)
    {
        PluginMisconfigurationExceptionHelper.ThrowIsNullOrEmpty(input.PageId, nameof(input.PageId));
        var result = await GetPage<GenericPageDto>(ApiEndpoints.ALandingPage(input.PageId));
        var activityInfo = await GetActivityInfo();

        var htmlFile = HtmlConverter.ToHtml(result.LayoutSections, result.HtmlTitle, result.Language, input.PageId, ContentTypes.LandingPage, Properties,
            result.Slug, result.Url, $"https://app.hubspot.com/pages/{activityInfo.PortalId}/editor/{input.PageId}/content",  result.MetaDescription, string.Empty);

        FileReference file;
        using (var stream = new MemoryStream(htmlFile))
        {
            file = await FileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{input.PageId}.html");
        }

        return new()
        {
            Content = file,
            FileLanguage = result.Language
        };
    }

    [Action("Translate landing page from HTML file",
        Description = "Create a new translation for a site page based on a file input")]
    public async Task<TranslationResponse> TranslateLandingPageFromFile(
        [ActionParameter] TranslateLandingPageFromFileRequest request)
    {
        var file = await FileManagementClient.DownloadAsync(request.File);
        var (pageInfo, json) = HtmlConverter.ToJson(file);

        var sourcePageId = request.SourcePageId ?? pageInfo.HtmlDocument.ExtractBlackbirdReferenceId() 
            ?? throw new Exception("Landing page ID not found. Please provide it in optional input");
        var primaryLanguage = string.IsNullOrEmpty(pageInfo.Language) ? request.PrimaryLanguage : pageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage))
        {
            throw new PluginMisconfigurationException(
                "You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        }        
        
        var translationId = await GetOrCreateTranslationId(ApiEndpoints.LandingPages, sourcePageId, request.TargetLanguage, primaryLanguage);

        var page = await UpdateTranslatedPage(ApiEndpoints.UpdateLandingPage(translationId), new()
        {
            Id = translationId,
            HtmlTitle = pageInfo.Title,
            LayoutSections = json
        });

        return new()
        {
            TranslationId = translationId,
            PageId = page.Id
        };
    }

    [Action("Schedule landing page for publishing",
        Description = "Schedules a landing page for publishing on the given time")]
    public Task ScheduleALandingPageForPublish([ActionParameter] PublishLandingPageRequest request)
    {
        var publishData = request.DateTime ?? DateTime.Now.AddSeconds(30);
        return PublishPage(ApiEndpoints.PublishLandingPage, request.Id, publishData);
    }
}