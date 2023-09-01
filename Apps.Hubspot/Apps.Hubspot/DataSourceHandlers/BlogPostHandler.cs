using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class BlogPostHandler : HubSpotInvocable, IAsyncDataSourceHandler
{
    public BlogPostHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var endpoint = $"/blogs/posts?name__icontains={context.SearchString}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        
        var blogPosts = await Client.ExecuteWithErrorHandling<GetAllResponse<BlogPostDto>>(request);

        return blogPosts.Results
            .ToDictionary(k => k.Id, v => v.Name);
    }
}