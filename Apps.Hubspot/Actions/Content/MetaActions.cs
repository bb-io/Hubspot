using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Models.Responses.Files;
using Apps.Hubspot.Services;
using Apps.Hubspot.Utils;
using Apps.Hubspot.Utils.Extensions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using System.Net.Mime;
using System.Text;

namespace Apps.Hubspot.Actions.Content;

[ActionList]
public class MetaActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : HubSpotInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext);

    [Action("Search content", Description = "Search for any type of content")]
    public async Task<ListResponse<Metadata>> SearchContent([ActionParameter] ContentTypesFilter typesFilter,
        [ActionParameter] LanguageFilter languageFilter,
        [ActionParameter] TimeFilterRequest timeFilter,
        [ActionParameter] SearchContentRequest searchContentRequest)
    {
        var contentServices = _factory.GetContentServices(typesFilter.ContentTypes);
        var timeQuery = timeFilter.AsQuery();
        var languageQuery = languageFilter.AsQuery();
        var searchContentQuery = searchContentRequest.AsQuery();
        
        var query = searchContentQuery.Combine(timeQuery,languageQuery,searchContentQuery);

        var metadata = await contentServices.ExecuteManyAsync(query);

        return new(metadata);
    }

    [Action("Get translation language codes",
        Description = "Returns list of translated locales for specific content based on ID")]
    public async Task<TranslatedLocalesResponse> GetTranslationLanguageCodes(
        [ActionParameter] GetContentForTranslationLanguageCodesRequest contentRequest)
    {
        PluginMisconfigurationExceptionHelper.ThrowIsNullOrEmpty(contentRequest.ContentType, nameof(contentRequest.ContentType));
        PluginMisconfigurationExceptionHelper.ThrowIsNullOrEmpty(contentRequest.ContentId, nameof(contentRequest.ContentId));

        var contentService = _factory.GetContentService(contentRequest.ContentType);
        return await contentService.GetTranslationLanguageCodesAsync(contentRequest.ContentId);
    }

    [Action("Get content", Description = "Retrieve metadata for a specific content type based on its ID")]
    public async Task<Metadata> GetContent([ActionParameter] GetContentRequest contentRequest)
    {
        var contentService = _factory.GetContentService(contentRequest.ContentType);
        return await contentService.GetContentAsync(contentRequest.ContentId);
    }

    [Action("Download content", Description = "Download content as HTML for a specific content type based on its ID")]
    public async Task<FileLanguageResponse> DownloadContent([ActionParameter] GetContentRequest contentRequest)
    {
        var content = await GetContent(contentRequest);
        var contentService = _factory.GetContentService(contentRequest.ContentType);
        var stream = await contentService.DownloadContentAsync(contentRequest.ContentId);
        var fileReference =
            await fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{content.Title}.html");

        return new()
        {
            File = fileReference,
            FileLanguage = content.Language
        };
    }

    [Action("Upload content", Description = "Update content from an HTML file")]
    public async Task<Metadata> UpdateContentFromHtml([ActionParameter] LanguageFileRequest languageFileRequest,
        [ActionParameter] UploadContentRequest uploadContentRequest)
    {
        var fileMemory = await fileManagementClient.DownloadAsync(languageFileRequest.File);
        var memoryStream = new MemoryStream();
        await fileMemory.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var fileBytes = await memoryStream.GetByteData();
        memoryStream.Position = 0;

        var fileString = Encoding.UTF8.GetString(fileBytes);
        var document = fileString.AsHtmlDocument();
        var contentType = document.ExtractContentType();

        var contentService = _factory.GetContentService(contentType);
        return await contentService.UpdateContentFromHtmlAsync(languageFileRequest.TargetLanguage, memoryStream, uploadContentRequest);
    }

    [Action("Update content", Description = "Update content based on specified criteria using its ID")]
    public async Task<Metadata> UpdateContent([ActionParameter] GetContentRequest contentRequest,
        [ActionParameter] UpdateContentRequest updateRequest)
    {
        var contentService = _factory.GetContentService(contentRequest.ContentType);
        return await contentService.UpdateContentAsync(contentRequest.ContentId, updateRequest);
    }

    [Action("Delete content", Description = "Delete content based on its ID")]
    public async Task DeleteContent([ActionParameter] GetContentRequest contentRequest)
    {
        var contentService = _factory.GetContentService(contentRequest.ContentType);
        await contentService.DeleteContentAsync(contentRequest.ContentId);
    }
}