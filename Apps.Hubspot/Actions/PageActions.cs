using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.SitePages;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Apps.Hubspot.Models.Responses.Translations;
using Apps.Hubspot.Services;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;
using Apps.Hubspot.Utils;
using Apps.Hubspot.Utils.Converters;

namespace Apps.Hubspot.Actions;

[ActionList]
public class PageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : BasePageActions(invocationContext, fileManagementClient)
{
    [Action("Search site pages", Description = "Search for a list of site pages that match a certain criteria")]
    public async Task<ListResponse<PageDto>> GetAllSitePages([ActionParameter] SearchPagesRequest searchPageRequest,
        [ActionParameter] SearchPagesAdditionalRequest additionalRequest, [ActionParameter][Display("Site name must be an exact match")]bool? exactMatch)
    {
        var searchPageQuery = searchPageRequest.AsHubspotQuery();

        var additionalQuery = additionalRequest.AsQuery();

        var query = searchPageQuery.Combine(additionalQuery);

        var endpoint = ApiEndpoints.SitePages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        if (searchPageRequest.NotTranslatedInLanguage != null)
        {
            response = response.Where(p => p.Translations == null || p.Translations.Keys.All(key => key != searchPageRequest.NotTranslatedInLanguage.ToLower())).ToList();
        }

        if (exactMatch.HasValue && exactMatch.Value && !String.IsNullOrEmpty(searchPageRequest.Name))
        {
            response = response.Where(x => x.Name == searchPageRequest.Name).ToList();
        }

        var items = response.Select(x => x.DeepClone()).ToList();

        return new(items);
    }

    [Action("Get site page translation language codes", Description = "Returns list of translated locales for site page based on ID")]
    public async Task<TranslatedLocalesResponse> GetListOfTranslatedLocales([ActionParameter] SitePageRequest request)
    {
        ContentServicesFactory factory = new(InvocationContext);
        var service = factory.GetContentService(ContentTypes.SitePage);
        return await service.GetTranslationLanguageCodesAsync(request.PageId);
    }

    [Action("Get a site page", Description = "Get information of a specific page")]
    public Task<PageDto> GetSitePage([ActionParameter] SitePageRequest input)
    {
        PluginMisconfigurationExceptionHelper.ThrowIsNullOrEmpty(input.PageId, nameof(input.PageId));
        return GetPage<PageDto>(ApiEndpoints.ASitePage(input.PageId));
    }

    [Action("Get a site page as HTML file",
        Description = "Get information of a specific page and return an HTML file of its content")]
    public async Task<FileLanguageResponse> GetSitePageAsHtml([ActionParameter] SitePageRequest input, 
        [ActionParameter] LocalizablePropertiesRequest Properties)
    {
        PluginMisconfigurationExceptionHelper.ThrowIsNullOrEmpty(input.PageId, nameof(input.PageId));

        var result = await GetPage<GenericPageDto>(ApiEndpoints.ASitePage(input.PageId));
        
        if(string.IsNullOrEmpty(result.Language))
        {
            throw new PluginMisconfigurationException("The page does not have a language set. Please set the language and try again");
        }

        var htmlFile =
            HtmlConverter.ToHtml(result.LayoutSections, result.HtmlTitle, result.Language, input.PageId, ContentTypes.SitePage, Properties);

        FileReference file;
        var title = result.HtmlTitle ?? result.Name;
        using (var stream = new MemoryStream(htmlFile))
        {
            file = await FileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{title}.html");
        }

        return new()
        {
            File = file,
            FileLanguage = result.Language,
        };
    }

    [Action("Translate a site page from HTML file",
        Description = "Create a new translation for a site page based on a file searchPageRequest")]
    public async Task<TranslationResponse> TranslateSitePageFromFile(
        [ActionParameter] TranslateSitePageFromFileRequest request)
    {
        if (request.File == null)
        {
            throw new PluginMisconfigurationException("The file searchPageRequest is not found. Please check the searchPageRequest and try again");
        }

        if (string.IsNullOrWhiteSpace(request.TargetLanguage))
        {
            throw new PluginMisconfigurationException("The target language is not found. Please check the searchPageRequest and try again");
        }

        var file = await FileManagementClient.DownloadAsync(request.File);
        var (pageInfo, json) = HtmlConverter.ToJson(file);

        var sourcePageId = request.SourcePageId ?? pageInfo.HtmlDocument.ExtractBlackbirdReferenceId() ?? throw new PluginMisconfigurationException("The source page ID is missing. Provide it as an optional searchPageRequest");
        var primaryLanguage = string.IsNullOrEmpty(pageInfo.Language) ? request.PrimaryLanguage : pageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage))
        {
            throw new PluginMisconfigurationException(
                "You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        }        
        
        var translationId = await GetOrCreateTranslationId(ApiEndpoints.SitePages, sourcePageId, request.TargetLanguage, primaryLanguage);

        var page = await UpdateTranslatedPage(ApiEndpoints.UpdatePage(translationId), new()
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

    [Action("Schedule a site-page for publishing",
        Description = "Schedules a site page for publishing on the given time")]
    public Task ScheduleASitePageForPublish([ActionParameter] PublishSitePageRequest request)
    {
        var dateTime = request.DateTime ?? DateTime.Now.AddSeconds(30);
        return PublishPage(ApiEndpoints.PublishPage, request.PageId, dateTime);
    }
}