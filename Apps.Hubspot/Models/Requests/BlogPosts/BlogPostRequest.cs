using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class BlogPostRequest
{
    [Display("Post ID")]
    [DataSource(typeof(BlogPostHandler))]
    public string BlogPostId { get; set; }
}