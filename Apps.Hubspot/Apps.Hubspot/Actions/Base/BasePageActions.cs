using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Pages;
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

    protected Task<PageDto> GetPage(string url)
    {
        var request = new HubspotRequest(url, Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<PageDto>(request);
    }

    protected Task<PageDto> CreateTranslation(string url,
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

        return Client.ExecuteWithErrorHandling<PageDto>(request);
    }

    protected Task<RestResponse> UpdateTranslatedPage(string url, BasePageDto page)
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