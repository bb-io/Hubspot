using System.Text;
using Apps.Hubspot.Helpers;
using Apps.Hubspot.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.SitePages;
using Apps.Hubspot.Models.Requests.Translations;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Apps.Hubspot.Models.Responses.Translations;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.Actions;

[ActionList]
public class PageActions : BasePageActions
{
    public PageActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get all site pages", Description = "Get a list of all pages")]
    public Task<GetAllResponse<PageDto>> GetAllSitePages()
    {
        var request = new HubspotRequest(ApiEndpoints.SitePages(), Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
    }

    [Action("Get all site pages with certain language",
        Description = "Get a list of all pages in a specified language")]
    public Task<GetAllResponse<PageDto>> GetAllSitePagesInLanguage([ActionParameter] LanguageRequest input)
    {
        var endpoint = $"{ApiEndpoints.SitePages()}?language={input.Language}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
    }


    [Action("Get all site pages not translated in certain language",
        Description = "Get a list of all pages not translated in specified language")]
    public async Task<GetAllResponse<PageDto>> GetAllSitePagesNotInLanguage(
        [ActionParameter] TranslationRequest input)
    {
        var endpoint = $"{ApiEndpoints.SitePages()}?property=language,id,translations";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var getAllResponse = await Client.ExecuteWithErrorHandling<GetAllResponse<GenericPageDto>>(request);

        var result = getAllResponse.Results
            .Where(p => p.Language is null || (p.Language == input.PrimaryLanguage
                                               && p.Translations
                                                   .Properties()
                                                   .All(t => t.Name != input.Language.ToLower())));
        return new()
        {
            Results = result.Cast<PageDto>().ToList()
        };
    }

    [Action("Get all site pages updated after datetime",
        Description =
            "Get a list of all site pages that were updated after the given date time. Date time is exclusive")]
    public Task<GetAllResponse<PageDto>> GetAllPagesAfter([ActionParameter] UpdatedAfterRequest input)
    {
        var endpoint = ApiEndpoints.SitePages(input.UpdatedAfter);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<GetAllResponse<PageDto>>(request);
    }

    [Action("Get a site page", Description = "Get information of a specific page")]
    public Task<PageDto> GetSitePage([ActionParameter] SitePageRequest input)
        => GetPage<PageDto>(ApiEndpoints.ASitePage(input.PageId));

    [Action("Get a site page as HTML file",
        Description = "Get information of a specific page and return an HTML file of its content")]
    public async Task<FileResponse> GetSitePageAsHtml([ActionParameter] SitePageRequest input)
    {
        var result = await GetPage<GenericPageDto>(ApiEndpoints.ASitePage(input.PageId));
        var htmlStringBuilder =
            PageHelpers.ObjectToHtml(result.LayoutSections);
        var htmlFile = (result.HtmlTitle, result.Language, htmlStringBuilder).AsPageHtml();

        return new()
        {
            File = new(Encoding.UTF8.GetBytes(htmlFile))
            {
                Name = $"{result.HtmlTitle}.html",
                ContentType = MediaTypeNames.Text.Html
            },
            FileLanguage = result.Language,
        };
    }

    [Action("Translate a site page from HTML file",
        Description = "Create a new translation for a site page based on a file input")]
    public async Task<TranslationResponse> TranslateSitePageFromFile(
        [ActionParameter] TranslateSitePageFromFileRequest request)
    {
        var pageInfo = PageHelpers.ExtractParentInfo(request.File.Bytes);
        var translationResponse = await CreateTranslation(ApiEndpoints.CreateTranslation,
            request.SourcePageId, pageInfo.Language, request.TargetLanguage);

        var translationId = translationResponse.Id;
        var updateResponse = await UpdateTranslatedPage(ApiEndpoints.UpdatePage(translationId), new()
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

    [Action("Schedule a site-page for publishing",
        Description = "Schedules a site page for publishing on the given time")]
    public Task ScheduleASitePageForPublish([ActionParameter] PublishSitePageRequest request)
    {
        var dateTime = request.DateTime ?? DateTime.Now.AddSeconds(30);
        return PublishPage(ApiEndpoints.PublishPage, request.PageId, dateTime);
    }
}