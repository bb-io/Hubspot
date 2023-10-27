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
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Hubspot.Actions;

[ActionList]
public class LandingPageActions : BasePageActions
{
    public LandingPageActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get all landing pages", Description = "Get a list of all landing pages")]
    public Task<GetAllResponse<PageDto>> GetAllLandingPages()
    {
        var request = new HubspotRequest(ApiEndpoints.LandingPages(), Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
    }


    [Action("Get all landing pages with certain language",
        Description = "Get a list of all pages in a specified language")]
    public Task<GetAllResponse<PageDto>> GetAllLandingPagesInLanguage([ActionParameter] LanguageRequest input)
    {
        var endpoint = $"{ApiEndpoints.LandingPages()}?language={input.Language}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
    }

    [Action("Get all landing pages not translated in certain language",
        Description = "Get a list of all pages not translated in specified language")]
    public async Task<GetAllResponse<PageDto>> GetAllLandingPagesNotInLanguage(
        [ActionParameter] TranslationRequest input)
    {
        var endpoint = $"{ApiEndpoints.LandingPages()}?property=language,id,translations";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var getAllResponse = await Client.ExecuteWithErrorHandling<GetAllResponse<GenericPageDto>>(request);

        var result = getAllResponse.Results
            .Where(p => p.Language is null
                        || (p.Language == input.PrimaryLanguage
                            && p.Translations
                                .Properties()
                                .All(t => t.Name != input.Language.ToLower())));

        return new()
        {
            Results = result.Cast<PageDto>().ToList()
        };
    }

    [Action("Get all landing pages updated after datetime",
        Description =
            "Get a list of all landing pages that were updated after the given date time. Date time is exclusive")]
    public Task<GetAllResponse<PageDto>> GetAllLandingPagesAfter([ActionParameter] UpdatedAfterRequest input)
    {
        var endpoint = ApiEndpoints.LandingPages(input.UpdatedAfter);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
    }

    [Action("Get a landing page", Description = "Get information of a specific landing page")]
    public Task<PageDto> GetLandingPage([ActionParameter] LandingPageRequest input)
        => GetPage<PageDto>(ApiEndpoints.ALandingPage(input.PageId));

    [Action("Get a landing page as HTML file",
        Description = "Get information of a specific landing page and return an HTML file of its content")]
    public async Task<FileResponse> GetLandingPageAsHtml([ActionParameter] LandingPageRequest input)
    {
        var result = await GetPage<GenericPageDto>(ApiEndpoints.ALandingPage(input.PageId));

        var htmlStringBuilder = PageHelpers.ObjectToHtml(result.LayoutSections);
        var htmlFile = (result.HtmlTitle, result.Language, htmlStringBuilder).AsPageHtml();

        return new()
        {
            File = new(Encoding.UTF8.GetBytes(htmlFile))
            {
                Name = $"{input.PageId}.html",
                ContentType = MediaTypeNames.Text.Html
            },
            FileLanguage = result.Language,
        };
    }

    [Action("Translate a landing page from HTML file",
        Description = "Create a new translation for a site page based on a file input")]
    public async Task<TranslationResponse> TranslateLandingPageFromFile(
        [ActionParameter] TranslateLandingPageFromFileRequest request)
    {
        var pageInfo = PageHelpers.ExtractParentInfo(request.File.Bytes);
        var translationResponse = await CreateTranslation(ApiEndpoints.CreateLandingPageTranslation,
            request.SourcePageId,
            pageInfo.Language,
            request.TargetLanguage);

        var translationId = translationResponse.Id;
        var updateResponse = await UpdateTranslatedPage(ApiEndpoints.UpdateLandingPage(translationId), new()
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