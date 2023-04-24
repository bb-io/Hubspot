namespace Apps.Hubspot.Dtos.Blogs;

public class PostDto
{
    public string FileName { get; set; }

    public string MimeType { get; set; }

    public byte[] ContentInBytes { get; set; }
}