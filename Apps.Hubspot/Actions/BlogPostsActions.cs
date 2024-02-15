using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using RestSharp;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Exceptions;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.BlogPosts;
using Apps.Hubspot.Models.Requests.Translations;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;

namespace Apps.Hubspot.Actions;

[ActionList]
public class BlogPostsActions : BasePageActions
{
    public BlogPostsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext, fileManagementClient)
    {
    }

    [Action("Get all blog posts", Description = "Get a list of all blog posts")]
    public async Task<ListResponse<BlogPostDto>> GetAllBlogPosts([ActionParameter] SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.BlogPostsSegment.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<BlogPostDto>(request);

        return new(response);
    }

    [Action("Get all blog posts not translated in certain language",
        Description = "Get a list of all blog posts not translated in specified language")]
    public async Task<ListResponse<BlogPostDto>> GetBlogPostsWithoutTranslations(
        [ActionParameter] TranslationRequest input)
    {
        var request = new HubspotRequest(ApiEndpoints.BlogPostsSegment, Method.Get, Creds);
        var posts = await Client.ExecuteWithErrorHandling<GetAllResponse<BlogPostWithTranslationsDto>>(request);

        var result = posts.Results
            .Where(p => p.TranslatedFromId is null && (p.Language is null
                                                       || (p.Language == input.PrimaryLanguage
                                                           && p.Translations?
                                                                   .Properties()
                                                                   .All(t => t.Name != input.Language.ToLower())
                                                               is
                                                               true)));
        return new(result);
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

        if (translationsObj is null || !translationsObj.ContainsKey(input.Language))
            throw new("No translation found");

        return translationsObj[input.Language]!.ToObject<TranslationDto>()!;
    }

    [Action("Get blog post as HTML file", Description = "Get blog post as HTML file")]
    public async Task<FileLanguageResponse> GetBlogPostAsHtml(
        [ActionParameter] GetBlogPostAsHtmlRequest input)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{input.BlogPost}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var blogPost = await Client.ExecuteWithErrorHandling<BlogPostDto>(request);
        var htmlFile = (blogPost.Name, blogPost.PostBody).AsHtml();

        FileReference file;
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(htmlFile)))
        {
            file = await FileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{blogPost.Name}.html");
        }

        return new FileLanguageResponse
        {
            File = file,
            FileLanguage = blogPost.Language,
        };
    }

    [Action("Translate blog post from HTML file", Description = "Translate blog post from HTML file")]
    public async Task<BlogPostDto> TranslateBlogPostFromHtml(
        [ActionParameter] TranslateBlogPostFromHtmlRequest input)
    {
        var file = await FileManagementClient.DownloadAsync(input.File);
        var fileBytes = await file.GetByteData();

        var fileString = Encoding.UTF8.GetString(fileBytes);
        var doc = fileString.AsHtmlDocument();

        var title = doc.GetTitle();
        var body = doc.DocumentNode.SelectSingleNode("/html/body").InnerHtml;

        string postId;
        try
        {
            var createdPost = await CreateBlogLanguageVariation(new()
            {
                PostId = input.BlogPostId,
                Language = input.Language
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
                Language = input.Language
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

    [Action("Schedule a blog post for publishing",
        Description = "Schedules a blog post for publishing on the given time")]
    public Task ScheduleABlogPostForPublish([ActionParameter] PublishBlogpostRequest request)
    {
        var publishData = request.DateTime ?? DateTime.Now.AddSeconds(30);
        return PublishPage($"{ApiEndpoints.BlogPostsSegment}/schedule", request.Id, publishData);
    }
}