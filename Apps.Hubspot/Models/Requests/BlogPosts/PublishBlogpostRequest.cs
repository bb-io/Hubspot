using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class PublishBlogpostRequest
{
    [Display("Blog post ID")]
    [DataSource(typeof(BlogPostHandler))]
    public string Id { get; set; }
	
    [Display("Date time")]
    public DateTime? DateTime { get; set; }
}