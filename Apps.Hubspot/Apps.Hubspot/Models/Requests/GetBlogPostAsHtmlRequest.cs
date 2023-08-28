using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Hubspot.Models.Requests
{
    public class GetBlogPostAsHtmlRequest
    {
        [DataSource(typeof(BlogPostHandler))]
        [Display("Blog post")]
        public string BlogPost { get; set; }
    }
}
