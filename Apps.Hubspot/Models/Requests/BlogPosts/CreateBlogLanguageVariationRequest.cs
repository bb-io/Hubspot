using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class CreateBlogLanguageVariationRequest
{
    [Display("Post ID")]
    public string Id { get; set; }

    [StaticDataSource(typeof(LanguageHandler))]
    public string Language { get; set; }
}