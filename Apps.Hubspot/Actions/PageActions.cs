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
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Hubspot.Actions;

[ActionList]
public class PageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BasePageActions(invocationContext, fileManagementClient)
{
    [Action("Search site pages", Description = "Search for a list of site pages that match a certain criteria")]
    public async Task<ListResponse<PageDto>> GetAllSitePages([ActionParameter] SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.SitePages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        if (input.NotTranslatedInLanguage != null)
        {
            response = response.Where(p => p.Translations == null || p.Translations.Keys.All(key => key != input.NotTranslatedInLanguage.ToLower())).ToList();
        }

        if (!string.IsNullOrEmpty(input.Language))
        {
            response = response.Where(x => x.Language == input.Language).ToList();
        }

        return new(response);
    }

    [Action("Get a site page", Description = "Get information of a specific page")]
    public Task<PageDto> GetSitePage([ActionParameter] SitePageRequest input)
        => GetPage<PageDto>(ApiEndpoints.ASitePage(input.PageId));

    [Action("Get a site page as HTML file",
        Description = "Get information of a specific page and return an HTML file of its content")]
    public async Task<FileLanguageResponse> GetSitePageAsHtml([ActionParameter] SitePageRequest input)
    {
        var result = await GetPage<GenericPageDto>(ApiEndpoints.ASitePage(input.PageId));
        var htmlFile =
            HtmlConverter.ToHtml(result.LayoutSections, result.HtmlTitle, result.Language, input.PageId, ContentTypes.SitePage);

        FileReference file;
        using (var stream = new MemoryStream(htmlFile))
        {
            file = await FileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{result.HtmlTitle}.html");
        }

        return new()
        {
            File = file,
            FileLanguage = result.Language,
        };
    }

    [Action("Translate a site page from HTML file",
        Description = "Create a new translation for a site page based on a file input")]
    public async Task<TranslationResponse> TranslateSitePageFromFile(
        [ActionParameter] TranslateSitePageFromFileRequest request)
    {
        var file = await FileManagementClient.DownloadAsync(request.File);
        var (pageInfo, json) = HtmlConverter.ToJson(file);

        var sourcePageId = request.SourcePageId ?? pageInfo.HtmlDocument.ExtractBlackbirdReferenceId() ?? throw new Exception("The source page ID is missing. Provide it as an optional input or add it to the HTML file");
        var primaryLanguage = string.IsNullOrEmpty(pageInfo.Language) ? request.PrimaryLanguage : pageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage)) throw new Exception("You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        var translationId = await GetOrCreateTranslationId(ApiEndpoints.SitePages, sourcePageId, request.TargetLanguage, primaryLanguage);

        await UpdateTranslatedPage(ApiEndpoints.UpdatePage(translationId), new()
        {
            Id = translationId,
            HtmlTitle = pageInfo.Title,
            LayoutSections = json
        });

        return new()
        {
            TranslationId = translationId
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