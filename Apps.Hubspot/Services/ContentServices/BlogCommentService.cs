using System.Text;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Blogs.Comments;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Blogs.Comments;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class BlogCommentService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    protected override HubspotClient Client { get; } = new(Urls.Api);

    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest)
    {
        var endpoint = ApiEndpoints.BlogCommentsSegment.WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<GetBlogCommentsResponse>(request);
        return response.Objects.Select(x => ConvertBlogCommentToMetadata(x)).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var blogComment = await GetBlogCommentByIdAsync(id);
        return ConvertBlogCommentToMetadata(blogComment);
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogCommentsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    public override Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        throw new PluginMisconfigurationException("This method is not implemented for blog comments.");
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var blogComment = await GetBlogCommentByIdAsync(id);
        var html = BlogCommentsHtmlConverter.ConvertToHtml(blogComment);

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

        var blogCommentObject = BlogCommentsHtmlConverter.ConvertFromHtml(fileString);
        if (blogCommentObject == null)
        {
            throw new InvalidOperationException("Failed to convert HTML to BlogCommentDto.");
        }

        var createdBlogComment = await CreateBlogCommentAsync(blogCommentObject);
        return ConvertBlogCommentToMetadata(createdBlogComment);
    }

    public override Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {
        throw new PluginMisconfigurationException("This method is not implemented for blog comments.");
    }

    private async Task<BlogCommentDto> CreateBlogCommentAsync(BlogCommentDto blogComment)
    {
        var endpoint = ApiEndpoints.BlogCommentsSegment;
        var request = new HubspotRequest(endpoint, Method.Post, Creds)
            .AddJsonBody(blogComment);
        return await Client.ExecuteWithErrorHandling<BlogCommentDto>(request);
    }

    private async Task<BlogCommentDto> GetBlogCommentByIdAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.BlogCommentsSegment}/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        return await Client.ExecuteWithErrorHandling<BlogCommentDto>(request);
    }

    private Metadata ConvertBlogCommentToMetadata(BlogCommentDto blogComment)
    {
        return new Metadata
        {
            ContentId = blogComment.Id,
            Title = blogComment.ContentTitle,
            Type = ContentTypes.BlogComment,
            State = blogComment.State,
            Published = blogComment.State == "APPROVED",
            Url = string.Empty,
            Slug = string.Empty,
            CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(blogComment.CreatedAt).DateTime,
            UpdatedAt = DateTimeOffset.FromUnixTimeMilliseconds(blogComment.CreatedAt).DateTime // Comments don't have UpdatedAt, using CreatedAt
        };
    }
}
