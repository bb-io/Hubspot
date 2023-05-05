using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Dtos.Blogs.Posts
{
    public class CreateNewBlogLanguageRequest
    {
        public string PostId { get; set; }

        public string Language { get; set; }
    }
}
