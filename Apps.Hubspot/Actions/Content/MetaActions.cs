using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Apps.Hubspot.Services;
using Apps.Hubspot.Utils;
using Apps.Hubspot.Utils.Extensions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff1;
using Blackbird.Filters.Xliff.Xliff2;
using System.Net.Mime;
using System.Text;
using Metadata = Apps.Hubspot.Models.Responses.Content.Metadata;

namespace Apps.Hubspot.Actions.Content;

[ActionList("Content")]
public class MetaActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : HubSpotInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext);

    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    [Action("Search content", Description = "Search for any type of content")]
    public async Task<ListResponse<Metadata>> SearchContent([ActionParameter] ContentTypesFilter typesFilter,
        [ActionParameter] LanguageFilter languageFilter,
        [ActionParameter] TimeFilterRequest timeFilter,
        [ActionParameter] SearchContentRequest searchContentRequest)
    {
        if (searchContentRequest.UpdatedByUserIdsWhitelist?.Any() == true && 
            searchContentRequest.UpdatedByUserIdsBlacklist?.Any() == true)
        {
            throw new PluginMisconfigurationException("You cannot specify both whitelist and blacklist for updated by user IDs. Please use only one of them.");
        }

        var contentServices = _factory.GetContentServices(typesFilter.ContentTypes);
        var timeQuery = timeFilter.AsQuery();
        var languageQuery = languageFilter.AsQuery();
        var searchContentQuery = searchContentRequest.AsQuery();

        if (searchContentRequest.UpdatedByUserIdsWhitelist?.Count() == 1)
        {
            searchContentQuery.Add("updatedById__eq", searchContentRequest.UpdatedByUserIdsWhitelist.First());
        }

        var query = searchContentQuery.Combine(timeQuery, languageQuery, searchContentQuery);
        var metadata = await contentServices.ExecuteManyAsync(query, searchContentRequest);

        if (searchContentRequest.UpdatedByUserIdsWhitelist?.Any() == true && searchContentRequest.UpdatedByUserIdsWhitelist.Count() > 1)
        {
            metadata = metadata.Where(m => !string.IsNullOrEmpty(m.UpdatedByUserId) && searchContentRequest.UpdatedByUserIdsWhitelist.Contains(m.UpdatedByUserId)).ToList();
        }

        if (searchContentRequest.UpdatedByUserIdsBlacklist?.Any() == true)
        {
            metadata = metadata.Where(m => string.IsNullOrEmpty(m.UpdatedByUserId) || !searchContentRequest.UpdatedByUserIdsBlacklist.Contains(m.UpdatedByUserId)).ToList();
        }

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

    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
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
            Content = fileReference,
            FileLanguage = content.Language
        };
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload content", Description = "Update content from an HTML file")]
    public async Task<Metadata> UpdateContentFromHtml(
        [ActionParameter] LanguageFileRequest languageFileRequest,
        [ActionParameter] UploadContentRequest uploadContentRequest)
    {
        var fileMemory = await fileManagementClient.DownloadAsync(languageFileRequest.Content);
        string fileString;
        using (var reader = new StreamReader(fileMemory, Encoding.UTF8))
            fileString = await reader.ReadToEndAsync();

        if (Xliff2Serializer.IsXliff2(fileString) || Xliff1Serializer.IsXliff1(fileString))
        {
            fileString = Transformation.Parse(fileString, languageFileRequest.Content.Name)
                                       .Target()
                                       .Serialize()
                           ?? throw new PluginMisconfigurationException("XLIFF did not contain files");
        }

        var document = fileString.AsHtmlDocument();
        var contentType = document.ExtractContentType();

        if (!string.IsNullOrEmpty(languageFileRequest.ContentId))
        {
            document.SetReferenceId(languageFileRequest.ContentId);
            fileString = document.DocumentNode.OuterHtml;
        }

        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(fileString));

        var contentService = _factory.GetContentService(contentType);
        return await contentService.UpdateContentFromHtmlAsync(languageFileRequest.Locale, memoryStream, uploadContentRequest);
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