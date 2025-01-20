using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Extensions;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Responses.Content;

namespace Apps.Hubspot.Actions.Content;

//[ActionList]
public class MetaActions(InvocationContext invocationContext) : HubSpotInvocable (invocationContext)
{
    [Action("Search content", Description = "Search for any type of content")]
    public async Task<ListResponse<Metadata>> SearchContent([ActionParameter] ContentTypeFilter typeFilter, [ActionParameter] LanguageFilter languageFilter, [ActionParameter] TimeFilterRequest timeFilter)
    {
        var response = new List<Metadata>();

        var query = timeFilter.AsQuery();
        var blogEndpoint = ApiEndpoints.BlogPostsSegment.WithQuery(query);

        var request = new HubspotRequest(blogEndpoint, Method.Get, Creds);
        var blogPosts = await Client.Paginate<BlogPostDto>(request);

        response.AddRange(blogPosts.Select(x => new Metadata { Type = "blog", Id = x.Id, Language = x.Language }));

        

        return new(response);
    }
}

