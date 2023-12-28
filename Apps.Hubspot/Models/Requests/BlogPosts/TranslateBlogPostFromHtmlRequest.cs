using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class TranslateBlogPostFromHtmlRequest
{
    [DataSource(typeof(BlogPostHandler))]
    [Display("Blog post")]
    public string BlogPostId { get; set; }

    public string Locale { get; set; }

    public FileReference File { get; set; }
}