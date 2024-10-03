using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class CreateNewBlogLanguageRequest
{
    [Display("Post ID")]
    [DataSource(typeof(BlogPostHandler))]
    public string PostId { get; set; }

    [StaticDataSource(typeof(LanguageHandler))]
    public string Language { get; set; }
}