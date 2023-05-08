using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class TranslateBlogPostFromHtmlRequest
    {
        public string BlogPostId { get; set; }

        public string Locale { get; set; }

        public byte[] File { get; set; }
    }
}
