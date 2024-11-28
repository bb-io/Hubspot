using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers;

public class BlogPostHandler : HubSpotInvocable, IAsyncDataSourceItemHandler
{
    public BlogPostHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var endpoint = $"/blogs/posts?name__icontains={context.SearchString}&translatedFromId__is_null&limit=20";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var blogPosts = await Client.ExecuteWithErrorHandling<GetAllResponse<BlogPostDto>>(request);

        return blogPosts.Results.Select(bp=> new DataSourceItem
        {
            Value = bp.Id,
            DisplayName = bp.Name
        });
    }
}