using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Requests.Emails;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
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
using Blackbird.Filters.Constants;
using Blackbird.Filters.Extensions;
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
    public async Task<FileLanguageResponse> DownloadContent([ActionParameter] GetContentRequest contentRequest, [ActionParameter] LocalizablePropertiesRequest properties)
    {
        var content = await GetContent(contentRequest);
        var contentService = _factory.GetContentService(contentRequest.ContentType);
        var stream = await contentService.DownloadContentAsync(contentRequest.ContentId, properties);
        var fileReference =
            await fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{content.Title.SanitizeFileName()}.html");

        return new()
        {
            Content = fileReference,
            FileLanguage = content.Language
        };
    }

    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    [Action("Upload content", Description = "Update content from a processed content file")]
    public async Task<MetadataWithContent> UpdateContentFromHtml(
        [ActionParameter] LanguageFileRequest languageFileRequest,
        [ActionParameter] UploadContentRequest uploadContentRequest)
    {
        if (string.IsNullOrWhiteSpace(languageFileRequest?.Locale))
            throw new PluginMisconfigurationException(
                "Target language is null or empty. Please check your input and try again.");

        var fileMemory = await fileManagementClient.DownloadAsync(languageFileRequest.Content);
        string fileString;
        using (var reader = new StreamReader(fileMemory, Encoding.UTF8))
            fileString = await reader.ReadToEndAsync();

        Transformation? transformation = null;
        if (Xliff2Serializer.IsXliff2(fileString) || Xliff1Serializer.IsXliff1(fileString))
        {
            transformation = Transformation.Parse(fileString, languageFileRequest.Content.Name);
            fileString = transformation.Target().Serialize() ?? throw new PluginMisconfigurationException("XLIFF did not contain files");
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
        var metadata = await contentService.UpdateContentFromHtmlAsync(languageFileRequest.Locale, memoryStream, uploadContentRequest);

        var result = new MetadataWithContent
        {
            Slug = metadata.Slug,
            State = metadata.State,
            Subject = metadata.Subject,
            CreatedAt = metadata.CreatedAt,
            UpdatedAt = metadata.UpdatedAt,
            ContentId = metadata.ContentId,
            Domain = metadata.Domain,
            Language = metadata.Language,
            Published = metadata.Published,
            AdminUrl = metadata.AdminUrl,
            Title = metadata.Title,
            TranslatedFromId = metadata.TranslatedFromId,
            Type = metadata.Type,
            UpdatedByUserId = metadata.UpdatedByUserId,
            Url = metadata.Url,
        };

        if (transformation is not null)
        {
            transformation.TargetSystemReference.ContentId = metadata.ContentId;
            transformation.TargetSystemReference.ContentName = metadata.Title;
            transformation.TargetSystemReference.AdminUrl = metadata.AdminUrl;
            transformation.TargetSystemReference.PublicUrl = metadata.Url;
            transformation.TargetSystemReference.SystemName = "Hubspot";
            transformation.TargetSystemReference.SystemRef = "https://www.hubspot.com/";
            transformation.TargetLanguage = metadata.Language;

            result.Content = await fileManagementClient.UploadAsync(transformation.Serialize().ToStream(), MediaTypes.Xliff, transformation.XliffFileName);
        }
        else
        {
            result.Content = languageFileRequest.Content;
        }

        return result;
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