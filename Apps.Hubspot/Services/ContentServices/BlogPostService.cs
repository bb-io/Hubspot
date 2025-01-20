using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses.Content;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class BlogPostService(InvocationContext invocationContext) : HubSpotInvocable(invocationContext), IContentService
{
    public async Task<List<Metadata>> SearchContentAsync(TimeFilterRequest filterRequest)
    {
        var query = filterRequest.AsQuery();
        var blogEndpoint = ApiEndpoints.BlogPostsSegment.WithQuery(query);

        var request = new HubspotRequest(blogEndpoint, Method.Get, Creds);
        var blogPosts = await Client.Paginate<BlogPostDto>(request);

        return blogPosts.Select(x => new Metadata
        {
            Id = x.Id,
            Type = ContentTypes.Blog,
            Language = x.Language
        }).ToList();
    }
}