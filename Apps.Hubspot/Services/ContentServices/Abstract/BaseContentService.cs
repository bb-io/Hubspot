using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices.Abstract;

public abstract class BaseContentService(InvocationContext invocationContext)
    : HubSpotInvocable(invocationContext), IContentService
{
    public abstract Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest);

    public abstract Task<Metadata> GetContentAsync(string id);
    public abstract Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id);

    public abstract Task<Stream> DownloadContentAsync(string id, LocalizablePropertiesRequest properties);

    public abstract Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest);

    public abstract Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest);

    public abstract Task DeleteContentAsync(string id);

    protected Task<TranslatedLocalesResponse> GetTranslatedLocalesResponse(string primaryLanguage,
        Dictionary<string, ObjectWithId> translations)
    {
        var locales = translations.Keys.Select(x => x).ToList();
        return Task.FromResult(new TranslatedLocalesResponse
            {
                PrimaryLanguage = primaryLanguage,
                TranslationLanguageCodes = locales,
                Translations = translations.Select(x => new Translation { Id = x.Value.Id, Slug = x.Value.Slug, LanguageCode = x.Key }).ToList()
            }
        );
    }

    protected Task<T> UpdateTranslatedPage<T>(string url, UpdateTranslatedPageRequest page)
    {
        var request = new HubspotRequest(url, Method.Patch, Creds)
            .WithJsonBody(page, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling<T>(request);
    }

    protected async Task<string> GetOrCreateTranslationId(string resourceUrlPart, string resourceId,
        string targetLanguage, string primaryLanguage = null)
    {
        var request = new HubspotRequest($"{resourceUrlPart}/{resourceId}", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ObjectWithTranslations>(request);

        if (!string.IsNullOrEmpty(response.TranslatedFromId))
        {
            resourceId = response.TranslatedFromId;
            var request2 = new HubspotRequest($"{resourceUrlPart}/{resourceId}", Method.Get, Creds);
            response = await Client.ExecuteWithErrorHandling<ObjectWithTranslations>(request2);
        }

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
}