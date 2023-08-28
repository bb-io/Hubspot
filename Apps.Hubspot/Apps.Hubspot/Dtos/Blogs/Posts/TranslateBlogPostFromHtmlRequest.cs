using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class TranslateBlogPostFromHtmlRequest
    {
        [DataSource(typeof(BlogPostHandler))]
        [Display("Blog post")]
        public string BlogPostId { get; set; }

        public string Locale { get; set; }

        public File File { get; set; }
    }
}
