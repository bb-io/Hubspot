using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.SitePages;
using Apps.Hubspot.Models.Requests.Translations;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Hubspot.Actions.Base;

public abstract class BasePageActions : HubSpotInvocable
{
    protected BasePageActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    protected Task<T> GetPage<T>(string url) where T : PageDto
    {
        var request = new HubspotRequest(url, Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<T>(request);
    }

    protected Task<GenericPageDto> CreateTranslation(string url,
        string pageId, string primaryLanguage, string targetLanguage)
    {
        var payload = new CreateTranslationRequest
        {
            Id = pageId,
            PrimaryLanguage = primaryLanguage,
            Language = targetLanguage
        };
        var request = new HubspotRequest(url, Method.Post, Creds)
            .WithJsonBody(payload, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling<GenericPageDto>(request);
    }

    protected Task<RestResponse> UpdateTranslatedPage(string url, UpdateTranslatedPageRequest page)
    {
        var request = new HubspotRequest(url, Method.Patch, Creds)
            .WithJsonBody(page, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling(request);
    }

    protected Task PublishPage(string requestUrl, string pageId, DateTime dateTime)
    {
        var payload = new PublishPageRequest
        {
            Id = pageId,
            PublishDate = dateTime
        };
        var request = new HubspotRequest(requestUrl, Method.Post, Creds)
            .WithJsonBody(payload, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling(request);
    }
}