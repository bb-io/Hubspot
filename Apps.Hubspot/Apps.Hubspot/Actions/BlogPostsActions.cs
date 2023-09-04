using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using RestSharp;
using System.Text;
using Newtonsoft.Json.Linq;
using File = Blackbird.Applications.Sdk.Common.Files.File;
using System.Net.Mime;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Exceptions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Requests.BlogPosts;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;

namespace Apps.Hubspot.Actions;

[ActionList]
public class BlogPostsActions : HubSpotInvocable
{
    public BlogPostsActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get all blog posts", Description = "Get a list of all blog posts")]
    public Task<GetAllResponse<BlogPostDto>> GetBlogPosts()
    {
        var request = new HubspotRequest(ApiEndpoints.BlogPostsSegment, Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<GetAllResponse<BlogPostDto>>(request);
    }

    [Action("Get blog post", Description = "Get information of a specific blog post")]
    public Task<BlogPostDto> GetBlogPost([ActionParameter] BlogPostRequest blogPost)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{blogPost.BlogPostId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<BlogPostDto>(request);
    }

    [Action("Create blog post", Description = "Create a new blog post")]
    public Task<BlogPostDto> CreateBlogPost([ActionParameter] ManageBlogPostRequest input)
    {
        var request = new HubspotRequest(ApiEndpoints.BlogPostsSegment, Method.Post, Creds)
            .WithJsonBody(input, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling<BlogPostDto>(request);
    }

    [Action("Create blog language variation", Description = "Create new blog language variation")]
    public Task<BlogPostDto> CreateBlogLanguageVariation(
        [ActionParameter] CreateNewBlogLanguageRequest input)
    {
        var payload = new CreateBlogLanguageVariationRequest
        {
            Id = input.PostId,
            Language = input.Language
        };
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/multi-language/create-language-variation";
        var request = new HubspotRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(payload, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling<BlogPostDto>(request);
    }

    [Action("Update blog post", Description = "Update a blog post information")]
    public Task<BlogPostDto> UpdateBlogPost(
        [ActionParameter] BlogPostRequest blogPost,
        [ActionParameter] ManageBlogPostRequest input)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{blogPost.BlogPostId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(input, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling<BlogPostDto>(request);
    }

    [Action("Delete blog post", Description = "Delete a blog post")]
    public Task DeleteBlogPost([ActionParameter] BlogPostRequest blogPost)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{blogPost.BlogPostId}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }

    [Action("Get blog post translation", Description = "Get blog post translation by language")]
    public async Task<TranslationDto> GetBlogPostTranslation(
        [ActionParameter] GetBlogPostTranslationRequest input)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{input.BlogPostId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling(request);

        var translationsObj = JObject.Parse(response.Content)["translations"].ToObject<JObject>();

        if (translationsObj is null || !translationsObj.ContainsKey(input.Locale))
            throw new("No translation found");

        return translationsObj[input.Locale]!.ToObject<TranslationDto>()!;
    }

    [Action("Get blog post as HTML file", Description = "Get blog post as HTML file")]
    public async Task<FileResponse> GetBlogPostAsHtml(
        [ActionParameter] GetBlogPostAsHtmlRequest input)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{input.BlogPost}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var blogPost = await Client.ExecuteWithErrorHandling<BlogPostDto>(request);
        var htmlFile = (blogPost.Name, blogPost.PostBody).AsHtml();

        return new FileResponse()
        {
            File = new File(Encoding.UTF8.GetBytes(htmlFile))
            {
                Name = $"{blogPost.Name}.html",
                ContentType = MediaTypeNames.Text.Html
            },
            FileLanguage = blogPost.Language,
        };
    }

    [Action("Translate blog post from HTML file", Description = "Translate blog post from HTML file")]
    public async Task<BlogPostDto> TranslateBlogPostFromHtml(
        [ActionParameter] TranslateBlogPostFromHtmlRequest input)
    {
        var fileString = Encoding.UTF8.GetString(input.File.Bytes);
        var doc = fileString.AsHtmlDocument();

        var title = doc.GetTitle();
        var body = doc.DocumentNode.SelectSingleNode("/html/body").InnerHtml;

        string postId;
        try
        {
            var createdPost = await CreateBlogLanguageVariation(new()
            {
                PostId = input.BlogPostId,
                Language = input.Locale
            });

            postId = createdPost.Id;
        }
        catch (HubspotException ex)
        {
            if (ex.ErrorType != ErrorTypes.LanguageAlreadyTranslated)
                throw;

            var existingTranslation = await GetBlogPostTranslation(new()
            {
                BlogPostId = input.BlogPostId,
                Locale = input.Locale
            });

            postId = existingTranslation.Id;
        }

        return await UpdateBlogPost(new()
        {
            BlogPostId = postId
        }, new()
        {
            Name = title,
            PostBody = body
        });
    }

    [Action("Get blog posts without translations",
        Description = "Get blog posts without translations to specific language")]
    public async Task<GetAllResponse<BlogPostDto>> GetBlogPostsWithoutTranslations(
        [ActionParameter] [Display("Locale")] string locale)
    {
        var posts = await GetBlogPosts();
        var missingTranslationsPosts = new List<BlogPostDto>();

        foreach (var post in posts.Results)
        {
            if (post.TranslatedFromId is not null)
                continue;

            var translation = await GetBlogPostTranslation(new()
            {
                BlogPostId = post.Id,
                Locale = locale
            });

            if (translation is null)
                missingTranslationsPosts.Add(post);
        }

        return new()
        {
            Results = missingTranslationsPosts
        };
    }
}