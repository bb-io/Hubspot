using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class GetBlogPostTranslationRequest : BlogPostRequest
{
    [StaticDataSource(typeof(LanguageHandler))]
    public string Language { get; set; }
}