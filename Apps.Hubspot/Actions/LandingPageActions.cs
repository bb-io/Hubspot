using RestSharp;
using System.Text;
using Apps.Hubspot.Helpers;
using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Apps.Hubspot.Models.Requests;
using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests.LandingPages;
using Apps.Hubspot.Models.Requests.Translations;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Apps.Hubspot.Models.Responses.Translations;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.String;

namespace Apps.Hubspot.Actions;

[ActionList]
public class LandingPageActions : BasePageActions
{
    public LandingPageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext, fileManagementClient)
    {
    }

    [Action("Get all landing pages", Description = "Get a list of all landing pages")]
    public async Task<ListResponse<PageDto>> GetAllLandingPages([ActionParameter] SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.LandingPages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<PageDto>(request);

        return new(response);
    }

    [Action("Get all landing pages not translated in certain language",
        Description = "Get a list of all pages not translated in specified language")]
    public async Task<ListResponse<PageDto>> GetAllLandingPagesNotInLanguage(
        [ActionParameter] TranslationRequest input)
    {
        var endpoint = $"{ApiEndpoints.LandingPages}?property=language,id,translations";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var getAllResponse = await Client.ExecuteWithErrorHandling<GetAllResponse<GenericPageDto>>(request);

        var result = getAllResponse.Results
            .Where(p => p.Language is null
                        || (p.Language == input.PrimaryLanguage
                            && p.Translations
                                .Properties()
                                .All(t => t.Name != input.Language.ToLower())));

        return new(result);
    }

    [Action("Get a landing page", Description = "Get information of a specific landing page")]
    public Task<PageDto> GetLandingPage([ActionParameter] LandingPageRequest input)
        => GetPage<PageDto>(ApiEndpoints.ALandingPage(input.PageId));

    [Action("Get a landing page as HTML file",
        Description = "Get information of a specific landing page and return an HTML file of its content")]
    public async Task<FileLanguageResponse> GetLandingPageAsHtml([ActionParameter] LandingPageRequest input)
    {
        var result = await GetPage<GenericPageDto>(ApiEndpoints.ALandingPage(input.PageId));

        var htmlStringBuilder = PageHelpers.ObjectToHtml(result.LayoutSections);
        var htmlFile = (result.HtmlTitle, result.Language, htmlStringBuilder).AsPageHtml();

        FileReference file;
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlFile)))
        {
            file = await FileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{input.PageId}.html");
        }

        return new()
        {
            File = file,
            FileLanguage = result.Language
        };
    }

    [Action("Translate a landing page from HTML file",
        Description = "Create a new translation for a site page based on a file input")]
    public async Task<TranslationResponse> TranslateLandingPageFromFile(
        [ActionParameter] TranslateLandingPageFromFileRequest request)
    {
        var file = await FileManagementClient.DownloadAsync(request.File);
        var fileBytes = await file.GetByteData();

        var pageInfo = PageHelpers.ExtractParentInfo(fileBytes);
        var translationResponse = await CreateTranslation(ApiEndpoints.CreateLandingPageTranslation,
            request.SourcePageId,
            pageInfo.Language,
            request.TargetLanguage);

        var translationId = translationResponse.Id;
        await UpdateTranslatedPage(ApiEndpoints.UpdateLandingPage(translationId), new()
        {
            Id = translationId,
            HtmlTitle = pageInfo.Title,
            LayoutSections = PageHelpers.HtmlToObject(pageInfo.Html, translationResponse.LayoutSections)
        });

        return new()
        {
            TranslationId = translationResponse.Id
        };
    }

    [Action("Schedule a landing page for publishing",
        Description = "Schedules a landing page for publishing on the given time")]
    public Task ScheduleALandingPageForPublish([ActionParameter] PublishLandingPageRequest request)
    {
        var publishData = request.DateTime ?? DateTime.Now.AddSeconds(30);
        return PublishPage(ApiEndpoints.PublishLandingPage, request.Id, publishData);
    }
}