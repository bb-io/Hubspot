using System.Text;
using Apps.Hubspot.Dtos.Blogs;
using Apps.Hubspot.Http;
using Apps.Hubspot.Models.Blogs;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Actions;

[ActionList]
public class BlogActions: BaseActions<Post, CreateOrUpdatePost>
{
    private readonly string _requestUrl = "https://api.hubapi.com/cms/v3/blogs/posts";
    
    public BlogActions() : base(new HttpRequestProvider())
    {
    }
    
    [Action("Get all posts", Description = "Get all posts from blog")]
    public GetAllPostDto GetCompanies(
        AuthenticationCredentialsProvider authenticationCredentialsProvider
    )
    {
        var result = GetAll(_requestUrl, null, authenticationCredentialsProvider).Select(CreatePostFileByEntity).ToList();
        return new GetAllPostDto {Results = result};
    }

    private PostDto CreatePostFileByEntity(Post post)
    {
        byte[] content = Encoding.UTF8.GetBytes(CreateHtmlContent(post));
        return new PostDto
        {
            FileName = post.Name + ".html",
            MimeType = "text/html",
            ContentInBytes = content
        };
    }

    private string CreateHtmlContent(Post post)
    {
        var sb = new StringBuilder("<!DOCTYPE html>");
        sb.AppendLine($"<html lang=\"{post.Language}\">");
        sb.AppendLine(post.HeadHtml);
        sb.AppendLine("<body>");
        sb.AppendLine("<h1>");
        sb.AppendLine(post.HtmlTitle);
        sb.AppendLine("</h1>");
        sb.AppendLine(post.PostBody);
        sb.AppendLine(post.FooterHtml);
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
}