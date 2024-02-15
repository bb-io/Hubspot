using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Models.Dtos.Blogs.Posts;

public class BlogPostWithTranslationsDto : BlogPostDto
{
    public JObject? Translations { get; set; }
}