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
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query)
    {
        var blogEndpoint = ApiEndpoints.BlogPostsSegment.WithQuery(query);

        var request = new HubspotRequest(blogEndpoint, Method.Get, Creds);
        var blogPosts = await Client.Paginate<BlogPostDto>(request);

        return blogPosts.Select(x => new Metadata
        {
            Id = x.Id,
            Title = x.Name,
            Domain = x.Domain,
            Type = ContentTypes.Blog,
            Language = x.Language!,
            State = x.CurrentState,
            Published = x.CurrentlyPublished,
            CreatedAt = StringToDateTimeConverter.ToDateTime(x.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(x.Updated)
        }).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {        
        var blogPost = await GetBlogPostAsync(id);

        return new()
        {
            Id = blogPost.Id,
            Title = blogPost.Name,
            Domain = blogPost.Domain,
            Language = blogPost.Language!,
            State = blogPost.CurrentState,
            Published = blogPost.CurrentlyPublished,
            Type = ContentTypes.Blog,
            CreatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Updated)
        };
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
        
        var blogPostId = document.ExtractBlackbirdReferenceId() ?? throw new PluginMisconfigurationException("Blog post ID not found in the file. Please, make sure you generated HTML file with our app");
        
        var title = document.GetTitle();
        var metaDescription = document.GetNodeFromHead("description");
        var body = document.DocumentNode.SelectSingleNode("/html/body").InnerHtml;

        var translationId = await GetOrCreateTranslationId(ApiEndpoints.BlogPostsSegment, blogPostId, targetLanguage);
        var blogPost = await UpdateFullBlogPostObjectAsync(new()
        {
            BlogPostId = translationId
        }, new()
        {
            Name = title,
            PostBody = body,
            MetaDescription = metaDescription
        });

        return new()
        {
            Id = blogPost.Id,
            Title = blogPost.Name,
            Domain = blogPost.Domain,
            Language = blogPost.Language!,
            State = blogPost.CurrentState,
            Published = blogPost.CurrentlyPublished,
            Type = ContentTypes.Blog,
            CreatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Updated)
        };
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
        return new()
        {
            Id = blogPost.Id,
            Title = blogPost.Name,
            Domain = blogPost.Domain,
            Language = blogPost.Language!,
            State = blogPost.CurrentState,
            Published = blogPost.CurrentlyPublished,
            Type = ContentTypes.Blog,
            CreatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Created),
            UpdatedAt = StringToDateTimeConverter.ToDateTime(blogPost.Updated)
        };
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
}