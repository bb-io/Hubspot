﻿using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class GetBlogPostAsHtmlRequest
{
    [DataSource(typeof(BlogPostHandler))]
    [Display("Post ID")]
    public string BlogPost { get; set; }
}