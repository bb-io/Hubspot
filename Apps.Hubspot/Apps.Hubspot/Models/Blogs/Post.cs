namespace Apps.Hubspot.Models.Blogs;

public class Post : BasePostProperties
{
    public string Id { get; set; }

    public string Url { get; set; }

    public string AuthorName { get; set; }

    public DateTime PublishDate { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    public DateTime DeletedAt { get; set; }

    public string Language { get; set; }

    public string HtmlTitle { get; set; }

    public string HeadHtml { get; set; }

    public string FooterHtml { get; set; }
}