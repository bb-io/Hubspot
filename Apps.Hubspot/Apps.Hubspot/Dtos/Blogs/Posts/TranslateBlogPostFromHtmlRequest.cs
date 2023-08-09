﻿using Apps.Hubspot.DynamicHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class TranslateBlogPostFromHtmlRequest
    {
        [DataSource(typeof(BlogPostHandler))]
        public string BlogPostId { get; set; }

        public string Locale { get; set; }

        public byte[] File { get; set; }
    }
}
