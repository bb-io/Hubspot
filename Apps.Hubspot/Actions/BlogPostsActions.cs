using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using RestSharp;
using System.Text;
using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.BlogPosts;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Apps.Hubspot.Utils.Extensions;
using HtmlExtensions = Blackbird.Applications.Sdk.Utils.Html.Extensions.HtmlExtensions;

namespace Apps.Hubspot.Actions;

[ActionList]
public class BlogPostsActions : BasePageActions
{
    public BlogPostsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
        : base(invocationContext, fileManagementClient)
    {
    }

    [Action("Search blog posts", Description = "Search for a list of blog posts matching certain criteria")]
    public async Task<ListResponse<BlogPostDto>> GetAllBlogPosts([ActionParameter] SearchPagesRequest input)
    {
        var query = input.AsQuery();
        var endpoint = ApiEndpoints.BlogPostsSegment.WithQuery(query);

        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var response = await Client.Paginate<BlogPostWithTranslationsDto>(request);

        if (input.NotTranslatedInLanguage != null)
        {
            response = response.Where(p => p.Translations == null || p.Translations.Keys.All(key => key != input.NotTranslatedInLanguage.ToLower())).ToList();
        }

        return new(response);
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

    [Action("Get blog post as HTML file", Description = "Get blog post as HTML file")]
    public async Task<FileLanguageResponse> GetBlogPostAsHtml(
        [ActionParameter] GetBlogPostAsHtmlRequest input)
    {
        var endpoint = $"{ApiEndpoints.BlogPostsSegment}/{input.BlogPost}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var blogPost = await Client.ExecuteWithErrorHandling<BlogPostDto>(request);
        var htmlFile = (blogPost.Name, blogPost.MetaDescription, blogPost.PostBody, input.BlogPost).AsHtml();

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
        var blogPostId = input.BlogPostId ?? doc.ExtractBlackbirdReferenceId() ?? throw new Exception("Blog post ID not found. Please provide it from optional input");
        
        var title = doc.GetTitle();
        var metaDescription = doc.GetNodeFromHead("description");
        var body = doc.DocumentNode.SelectSingleNode("/html/body").InnerHtml;

        var translationId = await GetOrCreateTranslationId(ApiEndpoints.BlogPostsSegment, blogPostId, input.Language);

        return await UpdateBlogPost(new()
        {
            BlogPostId = translationId
        }, new()
        {
            Name = title,
            PostBody = body,
            MetaDescription = metaDescription
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