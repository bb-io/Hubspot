using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class GetBlogPostTranslationRequest : BlogPostRequest
{
    [DataSource(typeof(LanguageHandler))]
    public string Language { get; set; }
}