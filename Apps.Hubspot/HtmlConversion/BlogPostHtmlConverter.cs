using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Blogs.Posts;
using Apps.Hubspot.Utils.Extensions;

namespace Apps.Hubspot.HtmlConversion;

public static class BlogPostHtmlConverter
{
    public static string ToHtml(this BlogPostDto blogPost)
    {
        var htmlFile = (blogPost.Name, blogPost.MetaDescription, blogPost.PostBody, blogPost.Id, blogPost.Slug, ContentTypes.Blog).AsHtml();
        return htmlFile;
    }
}