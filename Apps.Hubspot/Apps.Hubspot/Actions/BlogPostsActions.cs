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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

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
            return await client.GetAsync<GetAllResponse<BlogPostDto>>(request);
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

        [Action("Create blog language variation", Description = "Create new blog language variation")]
        public BlogPostDto? CreateBlogLanguageVariation(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateNewBlogLanguageRequest input)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts/multi-language/create-language-variation", Method.Post, authenticationCredentialsProviders);
            request.AddJsonBody(new
            {
                id = input.PostId,
                language = input.Language
            });
            return client.Post<BlogPostDto>(request);
        }

        [Action("Update blog post", Description = "Update a blog post information")]
        public BlogPostDto? UpdateCompany(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long blogPostId, [ActionParameter] CreateOrUpdateBlogPostDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts/{blogPostId}", Method.Patch, authenticationCredentialsProviders);
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

        [Action("Get blog post translation", Description = "Get blog post translation by language")]
        public TranslationInfoDto? GetBlogPostTranslation(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string blogPostId, [ActionParameter] string locale)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts/{blogPostId}", Method.Get, authenticationCredentialsProviders);

            var translationInfo = new TranslationInfoDto();
            JObject translationsObj = JObject.Parse(client.Get(request).Content)["translations"].ToObject<JObject>();
            if (translationsObj != null && translationsObj.ContainsKey(locale))
            {
                translationInfo = translationsObj[locale].ToObject<TranslationInfoDto>();
            }
            return translationInfo;
        }

        [Action("Update name and body of blog post", Description = "Update name and body of blog post")]
        public BlogPostDto? UpdateBlogPostNameAndBody(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string blogPostId, [ActionParameter] string name, [ActionParameter] string body)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts/{blogPostId}", Method.Patch, authenticationCredentialsProviders);
            request.AddJsonBody(new
            {
                name = name,
                postBody = body
            });
            return client.Patch<BlogPostDto>(request);
        }

        [Action("Get blog post as HTML file", Description = "Get blog post as HTML file")]
        public FileResponse GetBlogPostAsHtml(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string blogPostId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/cms/v3/blogs/posts/{blogPostId}", Method.Get, authenticationCredentialsProviders);
            var blogPost = client.Get<BlogPostDto>(request);
            string htmlFile = $"<html><head><title>{blogPost.Name}</title></head><body>{blogPost.PostBody}</body></html>";
            return new FileResponse()
            {
                File = Encoding.ASCII.GetBytes(htmlFile)
            };
        }

        [Action("Translate blog post from HTML file", Description = "Translate blog post from HTML file")]
        public BlogPostDto? TranslateBlogPostFromHtml(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] TranslateBlogPostFromHtmlRequest input)
        {
            var fileString = Encoding.ASCII.GetString(input.File);
            var doc = new HtmlDocument();
            doc.LoadHtml(fileString);
            var title = doc.DocumentNode.SelectSingleNode("html/head/title").InnerText;
            var body = doc.DocumentNode.SelectSingleNode("/html/body").InnerHtml;

            var existingTranslation = GetBlogPostTranslation(authenticationCredentialsProviders, input.BlogPostId, input.Locale);
            if(existingTranslation.Id == 0)
            {
                var createdPost = CreateBlogLanguageVariation(authenticationCredentialsProviders, new CreateNewBlogLanguageRequest()
                {
                    PostId = input.BlogPostId,
                    Language = input.Locale
                });
                return UpdateBlogPostNameAndBody(authenticationCredentialsProviders, createdPost.Id, title, body);
            }
            return UpdateBlogPostNameAndBody(authenticationCredentialsProviders, existingTranslation.Id.ToString(), title, body);
        }

        [Action("Get blog posts without translations", Description = "Get blog posts without translations to specific language")]
        public async Task<GetAllResponse<BlogPostDto>> GetBlogPostsWithoutTranslations(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string locale)
        {
            var posts = await GetBlogPosts(authenticationCredentialsProviders);
            var missingTranslationsPosts = new List<BlogPostDto>();
            foreach (var post in posts.Results)
            {
                if(post.TranslatedFromId == null)
                {
                    var translation = GetBlogPostTranslation(authenticationCredentialsProviders, post.Id, locale);
                    if(translation.Id == 0)
                    {
                        missingTranslationsPosts.Add(post);
                    }
                }
            }
            return new GetAllResponse<BlogPostDto>()
            {
                Results = missingTranslationsPosts
            };
        }
    }
}
