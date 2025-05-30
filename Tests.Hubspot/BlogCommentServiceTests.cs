using Apps.Hubspot.Services.ContentServices;
using Newtonsoft.Json;
using Tests.Hubspot.Base;
using System.Text;

namespace Tests.Hubspot;

[TestClass]
public class BlogCommentServiceTests : TestBase
{
    private readonly BlogCommentService _service;

    public BlogCommentServiceTests()
    {
        _service = new BlogCommentService(InvocationContext);
    }

    [TestMethod]
    public async Task SearchContentAsync_WithDefaultParameters_ShouldReturnComments()
    {
        var result = await _service.SearchContentAsync(new Dictionary<string, string>());

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task GetContentAsync_WithValidId_ShouldReturnComment()
    {
        // First get a valid ID from search
        var searchResult = await _service.SearchContentAsync(new Dictionary<string, string>());
        var validId = searchResult.FirstOrDefault()?.Id;
        
        Assert.IsNotNull(validId, "No comments found to test with");

        var result = await _service.GetContentAsync(validId);

        Assert.IsNotNull(result);
        Assert.AreEqual(validId, result.Id);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task DownloadContentAsync_WithValidId_ShouldReturnHtmlStream()
    {
        // Arrange
        string commentId = "412529979";
        
        // Act
        var stream = await _service.DownloadContentAsync(commentId);
        
        // Assert
        Assert.IsNotNull(stream);
        Assert.IsTrue(stream.Length > 0);
        
        // Read the stream to verify it contains HTML content
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        
        Assert.IsTrue(content.Contains("<html"), "Stream should contain HTML content");
        Assert.IsTrue(content.Length > 0, "Content should not be empty");
        
        Console.WriteLine($"Downloaded HTML content length: {content.Length}");
        Console.WriteLine($"First 200 characters: {content.Substring(0, Math.Min(200, content.Length))}");
    }
}
