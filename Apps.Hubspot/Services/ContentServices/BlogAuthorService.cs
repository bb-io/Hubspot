using System.Text;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Blogs.Authors;
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

public class BlogAuthorService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest)
    {
        var endpoint = ApiEndpoints.BlogAuthorsSegment.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var blogAuthors = await Client.Paginate<BlogAuthorDto>(request);
        return blogAuthors.Select(x => ConvertBlogAuthorToMetadata(x)).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var blogAuthor = await GetBlogAuthorByIdAsync(id);
        return ConvertBlogAuthorToMetadata(blogAuthor);
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogAuthorsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    public override Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        throw new PluginMisconfigurationException("This method is not implemented for blog authors.");
    }

    public override async Task<Stream> DownloadContentAsync(string id, LocalizablePropertiesRequest properties)
    {
        var blogAuthor = await GetBlogAuthorByIdAsync(id);
        var html = BlogAuthorsHtmlConverter.ConvertToHtml(blogAuthor);

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(html))
        {
            Position = 0
        };
        return stream;
    }

    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var fileBytes = await stream.GetByteData();
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var blogAuthorObject = BlogAuthorsHtmlConverter.ConvertFromHtml(fileString);
        if (blogAuthorObject == null)
        {
            throw new InvalidOperationException("Failed to convert HTML to BlogAuthorDto.");
        }

        var allBlogAuthors = await GetBlogAuthorsAsync(new Dictionary<string, string>
        {
            { "language", targetLanguage }
        });

        var authorWithTargetLanguage = allBlogAuthors.FirstOrDefault(x => x.TranslatedFromId == blogAuthorObject.Id);
        if (authorWithTargetLanguage == null)
        {
            var createdVariation = await CreateLanguageVariationAsync(blogAuthorObject, targetLanguage);
            return ConvertBlogAuthorToMetadata(createdVariation);
        }
        else
        {
            var endpoint = $"/blogs/authors/{authorWithTargetLanguage.Id}";
            var data = new
            {
                website = blogAuthorObject.Website,
                displayName = blogAuthorObject.DisplayName,
                facebook = blogAuthorObject.Facebook,
                fullName = blogAuthorObject.FullName,
                bio = blogAuthorObject.Bio,
                linkedin = blogAuthorObject.Linkedin,
                avatar = blogAuthorObject.Avatar,
                twitter = blogAuthorObject.Twitter,
                name = blogAuthorObject.Name,
                email = blogAuthorObject.Email,
                slug = blogAuthorObject.Slug
            };

            var request = new HubspotRequest(endpoint, Method.Patch, Creds)
                .AddJsonBody(data);
            var updatedAuthor = await Client.ExecuteWithErrorHandling<BlogAuthorDto>(request);
            return ConvertBlogAuthorToMetadata(updatedAuthor);
        }
    }

    public override Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {
        throw new PluginMisconfigurationException("This method is not implemented for blog authors.");
    }

    private async Task<BlogAuthorDto> CreateLanguageVariationAsync(BlogAuthorDto blogAuthor, string targetLanguage)
    {
        var endpoint = "/blogs/authors/multi-language/create-language-variation";
        var request = new HubspotRequest(endpoint, Method.Post, Creds)
            .AddJsonBody(new
            {
                language = targetLanguage,
                id = blogAuthor.Id,
                primaryLanguage = blogAuthor.Language,
                blogAuthor
            });

        return await Client.ExecuteWithErrorHandling<BlogAuthorDto>(request);
    }

    private async Task<List<BlogAuthorDto>> GetBlogAuthorsAsync(Dictionary<string, string> query)
    {
        var endpoint = ApiEndpoints.BlogAuthorsSegment.WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        return await Client.Paginate<BlogAuthorDto>(request);
    }

    private async Task<BlogAuthorDto> GetBlogAuthorByIdAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogAuthorsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<BlogAuthorDto>(request);
    }

    private Metadata ConvertBlogAuthorToMetadata(BlogAuthorDto blogAuthor)
    {
        return new Metadata
        {
            ContentId = blogAuthor.Id,
            Title = blogAuthor.Name,
            Domain = string.Empty, // Blog authors don't have domain
            Type = ContentTypes.BlogAuthor,
            Language = blogAuthor.Language,
            State = string.Empty, // Blog authors don't have a state field
            Published = true, // Authors are always considered published
            Url = string.Empty, // Blog authors don't have a URL
            Slug = blogAuthor.Slug,
            CreatedAt = StringToDateTimeConverter.ToDateTime(blogAuthor.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(blogAuthor.Updated),
            UpdatedByUserId = string.Empty // Blog authors don't have an updatedById field
        };
    }
}
