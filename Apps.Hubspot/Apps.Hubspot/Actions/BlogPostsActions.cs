using Apps.Hubspot.Dtos.Companies;
using Apps.Hubspot.Models.Companies;
using Apps.Hubspot.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Hubspot.Dtos.Blogs.Posts;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class BlogPostsActions
    {
        [Action("Get all blog posts", Description = "Get a list of all blog posts")]
        public async Task<GetAllResponse<BlogPostDto>> GetBlogPosts(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest("/cms/v3/blogs/posts", Method.Get, authenticationCredentialsProviders);
            return client.Get<GetAllResponse<BlogPostDto>>(request);
        }

        [Action("Get blog post", Description = "Get information of a specific blog post")]
        public BlogPostDto? GetBlogPost(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long blogPostId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts/{blogPostId}", Method.Get, authenticationCredentialsProviders);
            return client.Get<BlogPostDto>(request);
        }

        [Action("Create blog post", Description = "Create a new blog post")]
        public BlogPostDto? CreateBlogPost(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateOrUpdateBlogPostDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts", Method.Post, authenticationCredentialsProviders);
            request.AddJsonBody(dto);
            return client.Post<BlogPostDto>(request);
        }

        [Action("Update blog post", Description = "Update a blog post information")]
        public BlogPostDto? UpdateCompany(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long blogPostId, [ActionParameter] CreateOrUpdateBlogPostDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts{blogPostId}", Method.Patch, authenticationCredentialsProviders);
            request.AddJsonBody(dto);
            return client.Patch<BlogPostDto>(request);
        }

        [Action("Delete blog post", Description = "Delete a blog post")]
        public void DeleteCompany(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long blogPostId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts/{blogPostId}", Method.Delete, authenticationCredentialsProviders);
            client.Execute(request);
        }
    }
}
