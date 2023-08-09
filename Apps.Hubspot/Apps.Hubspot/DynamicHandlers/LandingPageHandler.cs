using Apps.Hubspot.Dtos.Blogs.Posts;
using Apps.Hubspot.Dtos.Pages;
using Apps.Hubspot.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.DynamicHandlers
{
    public class LandingPageHandler : BaseInvocable, IAsyncDataSourceHandler
    {
        public LandingPageHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var contextInv = InvocationContext;
            var client = new HubspotClient(contextInv.AuthenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/pages/landing-pages?name__icontains={context.SearchString}", Method.Get, contextInv.AuthenticationCredentialsProviders);
            var landingPages = await client.GetAsync<GetAllResponse<PageDto>>(request);
            return landingPages.Results.ToDictionary(k => k.Id, v => v.Name);
        }
    }
}
