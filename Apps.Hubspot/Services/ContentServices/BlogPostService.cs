using System.Text;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Requests.BlogPosts;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Apps.Hubspot.Utils.Converters;
using Apps.Hubspot.Utils.Extensions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class BlogPostService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest)
    {
        string language = null;
        if (query.TryGetValue("language", out string? value))
        {
            language = value;
            query.Remove("language");
        }
        else if(query.TryGetValue("Language", out value))
        {
            language = value;
            query.Remove("Language");
        }

        var blogEndpoint = ApiEndpoints.BlogPostsSegment.WithQuery(query);

        var request = new HubspotRequest(blogEndpoint, Method.Get, Creds);
        var blogPosts = await Client.Paginate<BlogPostDto>(request);
        if (!string.IsNullOrEmpty(searchContentRequest.UrlContains))
        {
            blogPosts = blogPosts.Where(x => x.Url.Contains(searchContentRequest.UrlContains, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (!string.IsNullOrEmpty(language))
        {
            blogPosts = blogPosts.Where(x => x.Language?.Equals(language, StringComparison.OrdinalIgnoreCase) == true).ToList();
        }

        return blogPosts.Select(ConvertBlogPostToMetadata).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var blogPost = await GetBlogPostAsync(id);
        return ConvertBlogPostToMetadata(blogPost);
    }

    public async Task<BlogPostDto> GetBlogPostAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<BlogPostDto>(request);
    }

    public override async Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var blogPost = await Client.ExecuteWithErrorHandling<BlogPostDto>(request);
        return await GetTranslatedLocalesResponse(blogPost.Language ?? string.Empty, blogPost.Translations);
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var blogPost = await Client.ExecuteWithErrorHandling<BlogPostDto>(request);
        var htmlFile = blogPost.ToHtml();

        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlFile));
        memoryStream.Position = 0;
        return memoryStream;
    }

    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var fileBytes = await stream.GetByteData();
        var fileString = Encoding.UTF8.GetString(fileBytes);
        var document = fileString.AsHtmlDocument();

        var blogPostId = document.ExtractBlackbirdReferenceId()
            ?? throw new PluginMisconfigurationException("Blog post ID not found in the file. Please, make sure you generated HTML file with our app");

        var postRequest = new ManageBlogPostRequest
        {
            Name = document.GetTitle(),
            PostBody = document.DocumentNode.SelectSingleNode("/html/body").InnerHtml
        };

        if (uploadContentRequest.UpdatePageMetdata == true)
        {
            postRequest.MetaDescription = document.GetNodeFromHead("description");
            postRequest.Slug = document.GetSlug();
        }

        var translationId = await GetOrCreateTranslationId(ApiEndpoints.BlogPostsSegment, blogPostId, targetLanguage);
        var blogPost = await UpdateFullBlogPostObjectAsync(new()
        {
            BlogPostId = translationId
        }, postRequest);

        return ConvertBlogPostToMetadata(blogPost);
    }

    public override async Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = updateContentRequest.Title
            }, JsonConfig.Settings);

        var blogPost = await Client.ExecuteWithErrorHandling<BlogPostDto>(request);
        return ConvertBlogPostToMetadata(blogPost);
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    private Task<BlogPostDto> UpdateFullBlogPostObjectAsync(BlogPostRequest blogPost, ManageBlogPostRequest input)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{blogPost.BlogPostId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(input, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling<BlogPostDto>(request);
    }
    
    private Metadata ConvertBlogPostToMetadata(BlogPostDto blogPost)
    {
        return new Metadata
        {
            Id = blogPost.Id,
            Title = blogPost.Name,
            Domain = blogPost.Domain,
            Language = blogPost.Language!,
            State = blogPost.CurrentState,
            Published = blogPost.CurrentlyPublished,
            Type = ContentTypes.Blog,
            Slug = blogPost.Slug,
            Url = blogPost.Url,
            CreatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Updated)
        };
    }
}