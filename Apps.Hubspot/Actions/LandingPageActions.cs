﻿using RestSharp;
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
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.String;

namespace Apps.Hubspot.Actions;

[ActionList]
public class LandingPageActions : BasePageActions
{
    public LandingPageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext, fileManagementClient)
    {
    }

    [Action("Search landing pages", Description = "Search for a list of site pages that match a certain criteria")]
    public async Task<ListResponse<PageDto>> GetAllLandingPages([ActionParameter] SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.LandingPages.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);

        if (input.NotTranslatedInLanguage != null)
        {
            response = response.Where(p => p.Translations == null || p.Translations.Keys.All(key => key != input.NotTranslatedInLanguage.ToLower())).ToList();
        }

        return new(response);
    }

    [Action("Get a landing page", Description = "Get information of a specific landing page")]
    public Task<PageDto> GetLandingPage([ActionParameter] LandingPageRequest input)
        => GetPage<PageDto>(ApiEndpoints.ALandingPage(input.PageId));

    [Action("Get a landing page as HTML file",
        Description = "Get information of a specific landing page and return an HTML file of its content")]
    public async Task<FileLanguageResponse> GetLandingPageAsHtml([ActionParameter] LandingPageRequest input)
    {
        var result = await GetPage<GenericPageDto>(ApiEndpoints.ALandingPage(input.PageId));

        var htmlFile = HtmlConverter.ToHtml(result.LayoutSections, result.HtmlTitle, result.Language, input.PageId);

        FileReference file;
        using (var stream = new MemoryStream(htmlFile))
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
        var (pageInfo, json) = HtmlConverter.ToJson(file);

        var primaryLanguage = string.IsNullOrEmpty(pageInfo.Language) ? request.PrimaryLanguage : pageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage)) throw new Exception("You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        var translationId = await GetOrCreateTranslationId(ApiEndpoints.LandingPages, request.SourcePageId, request.TargetLanguage, primaryLanguage);

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

    [Action("Schedule a landing page for publishing",
        Description = "Schedules a landing page for publishing on the given time")]
    public Task ScheduleALandingPageForPublish([ActionParameter] PublishLandingPageRequest request)
    {
        var publishData = request.DateTime ?? DateTime.Now.AddSeconds(30);
        return PublishPage(ApiEndpoints.PublishLandingPage, request.Id, publishData);
    }
}