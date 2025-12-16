using System.Text;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Blogs.Tags;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class BlogTagService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest)
    {
        var endpoint = "/blogs/tags".WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var blogTags = await Client.Paginate<BlogTagDto>(request);
        return blogTags.Select(x => ConvertBlogTagToMetadata(x)).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var blogTag = await GetBlogTagByIdAsync(id);
        return ConvertBlogTagToMetadata(blogTag);
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"/blogs/tags/{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    public override Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        throw new PluginMisconfigurationException("This method is not implemented for blog tags.");
    }

    public override async Task<Stream> DownloadContentAsync(string id, LocalizablePropertiesRequest properties)
    {
        var blogTag = await GetBlogTagByIdAsync(id);
        var html = BlogTagsHtmlConverter.ConvertToHtml(blogTag);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(html))
        {
            Position = 0
        };
        return stream;
    }    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var fileBytes = await stream.GetByteData();
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var blogTagObject = BlogTagsHtmlConverter.ConvertFromHtml(fileString);
        if (blogTagObject == null)
        {
            throw new InvalidOperationException("Failed to convert HTML to BlogTagDto.");
        }

        var allBlogTags = await GetBlogTagsAsync(new Dictionary<string, string>
        {
            { "language", targetLanguage }
        });

        var tagWithTargetLanguage = allBlogTags.FirstOrDefault(x => x.TranslatedFromId == blogTagObject.Id);
        if (tagWithTargetLanguage == null)
        {
            var createdVariation = await CreateLanguageVariationAsync(blogTagObject, targetLanguage);
            return ConvertBlogTagToMetadata(createdVariation);
        }
        else
        {
            var endpoint = $"/blogs/tags/{tagWithTargetLanguage.Id}";
            var data = new
            {
                name = blogTagObject.Name,
                slug = blogTagObject.Slug
            };

            var request = new HubspotRequest(endpoint, Method.Patch, Creds)
                .AddJsonBody(data);
            var updatedTag = await Client.ExecuteWithErrorHandling<BlogTagDto>(request);
            return ConvertBlogTagToMetadata(updatedTag);
        }
    }

    public override Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {
        throw new PluginMisconfigurationException("This method is not implemented for blog tags.");
    }

    private async Task<BlogTagDto> GetBlogTagByIdAsync(string id)
    {
        var endpoint = $"/blogs/tags/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<BlogTagDto>(request);
    }

    private Metadata ConvertBlogTagToMetadata(BlogTagDto blogTag)
    {
        return new Metadata
        {
            ContentId = blogTag.Id,
            Title = blogTag.Name,
            Domain = string.Empty, // Blog tags don't have domain
            Type = ContentTypes.BlogTag,
            Language = blogTag.Language,
            State = string.Empty, // Blog tags don't have a state field
            Published = true, // Tags are always considered published
            Url = string.Empty, // Blog tags don't have a URL
            Slug = blogTag.Slug,
            CreatedAt = StringToDateTimeConverter.ToDateTime(blogTag.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(blogTag.Updated),
            UpdatedByUserId = string.Empty
        };
    }

    private async Task<List<BlogTagDto>> GetBlogTagsAsync(Dictionary<string, string> query)
    {
        var endpoint = "/blogs/tags".WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        return await Client.Paginate<BlogTagDto>(request);
    }

    private async Task<BlogTagDto> CreateLanguageVariationAsync(BlogTagDto blogTag, string targetLanguage)
    {
        var endpoint = "/blogs/tags/multi-language/create-language-variation";
        var request = new HubspotRequest(endpoint, Method.Post, Creds)
            .AddJsonBody(new
            {
                language = targetLanguage,
                id = blogTag.Id,
                primaryLanguage = blogTag.Language,
                name = blogTag.Name,
                slug = blogTag.Slug
            });

        return await Client.ExecuteWithErrorHandling<BlogTagDto>(request);
    }
}
