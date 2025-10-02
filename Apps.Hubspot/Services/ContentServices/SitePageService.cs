using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;
using RestSharp;
using System.Text;

namespace Apps.Hubspot.Services.ContentServices;

public class SitePageService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest)
    {
        var endpoint = ApiEndpoints.SitePages.WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<GenericPageDto>(request);
        if(!string.IsNullOrEmpty(searchContentRequest.UrlContains))
        {
            response = response.Where(page => page.Url?.Contains(searchContentRequest.UrlContains, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
        }

        return response.Select(ConvertPageToMetadata).ToList();
    }
    
    public override async Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageWithTranslationsDto>(request);
        return await GetTranslatedLocalesResponse(page.Language ?? string.Empty, page.Translations);    
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);
        return ConvertPageToMetadata(page);
    }

    public async Task<PageWithTranslationsDto> GetPageAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<PageWithTranslationsDto>(request);
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Get, Creds);
        var page = await Client.ExecuteWithErrorHandling<GenericPageDto>(request);
        
        var htmlBytes =
            HtmlConverter.ToHtml(page.LayoutSections, page.HtmlTitle, page.Language, id, ContentTypes.SitePage, null, page.Slug, page.MetaDescription, string.Empty);
        return new MemoryStream(htmlBytes);
    }

    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var resultEntity = await HtmlConverter.ToJsonAsync(targetLanguage, stream, uploadContentRequest, InvocationContext);
        string sourcePageId = await GetSourcePageId(uploadContentRequest, resultEntity.PageInfo.HtmlDocument);
        var content = await GetContentAsync(sourcePageId);
        var primaryLanguage = !string.IsNullOrEmpty(content.Language) ? content.Language : resultEntity.PageInfo.Language;
        if (string.IsNullOrEmpty(primaryLanguage))
        {
            throw new PluginMisconfigurationException("You are creating a new multi-language variation of a page that has no primary language configured. Please select the primary language optional value");
        }

        var translationId = await GetOrCreateTranslationId(ApiEndpoints.SitePages, sourcePageId, targetLanguage, primaryLanguage);
        var updatedPage = new UpdateTranslatedPageRequest()
        {
            Id = translationId,
            HtmlTitle = resultEntity.PageInfo.Title,
            LayoutSections = resultEntity.Json
        };
        if (uploadContentRequest.UpdatePageMetdata.HasValue && uploadContentRequest.UpdatePageMetdata.Value)
        {
            
            stream.Position = 0;
            var fileBytes = await stream.GetByteData();

            var fileString = Encoding.UTF8.GetString(fileBytes);
            var document = fileString.AsHtmlDocument();

            var slug = document.ExtractSlug();
            var metaDescription = document.ExtractMetaDescription();

            if (!String.IsNullOrEmpty(slug))
            {
                updatedPage.Slug = slug;
            }
            if (!String.IsNullOrEmpty(metaDescription))
            {
                updatedPage.metaDescription = metaDescription;
            }
        }

        var pageDto = await UpdateTranslatedPage<PageDto>(ApiEndpoints.UpdatePage(translationId), updatedPage);
        return ConvertPageToMetadata(pageDto);
    }

    public override async Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {        
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = updateContentRequest.Title,
            });
        
        var page = await Client.ExecuteWithErrorHandling<PageDto>(request);
        return ConvertPageToMetadata(page);
    }

    public override Task DeleteContentAsync(string id)
    {        
        var url = ApiEndpoints.ASitePage(id);
        var request = new HubspotRequest(url, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    private Metadata ConvertPageToMetadata(PageDto page)
    {
        return new Metadata
        {
            ContentId = page.Id,
            Title = page.Name,
            Domain = page.Domain,
            Language = page.Language!,
            State = page.CurrentState,
            Published = page.Published,
            Type = ContentTypes.SitePage,
            Url = page.Url,
            Slug = page.Slug,
            CreatedAt = StringToDateTimeConverter.ToDateTime(page.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(page.Updated),
            TranslatedFromId = page.TranslatedFromId
        };
    }

    private async Task<string> GetSourcePageId(UploadContentRequest uploadContentRequest, HtmlDocument htmlDocument)
    {
        string sourcePageId;
        if (!string.IsNullOrEmpty(uploadContentRequest.SitePageId))
        {
            sourcePageId = uploadContentRequest.SitePageId;
        }
        else
        {
            var currentPageId = htmlDocument.ExtractBlackbirdReferenceId();

            var url = ApiEndpoints.ASitePage(currentPageId);
            var request = new HubspotRequest(url, Method.Get, Creds);
            var page = await Client.ExecuteWithErrorHandling<PageDto>(request);

            if (!string.IsNullOrEmpty(page.TranslatedFromId))
            {
                sourcePageId = page.TranslatedFromId;
            }
            else
            {
                sourcePageId = currentPageId;
            }
        }

        return sourcePageId;
    }
}