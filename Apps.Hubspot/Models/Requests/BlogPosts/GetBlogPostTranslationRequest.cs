namespace Apps.Hubspot.Models.Requests.BlogPosts;

public class GetBlogPostTranslationRequest : BlogPostRequest
{
    public string Locale { get; set; }
}