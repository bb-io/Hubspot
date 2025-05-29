using Apps.Hubspot.Services.ContentServices;
using Newtonsoft.Json;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class BlogAuthorServiceTests : TestBase
{
    private readonly BlogAuthorService _service;

    public BlogAuthorServiceTests()
    {
        _service = new BlogAuthorService(InvocationContext);
    }

    [TestMethod]
    public async Task SearchContentAsync_WithDefaultParameters_ShouldReturnAuthors()
    {
        var result = await _service.SearchContentAsync(new Dictionary<string, string>());

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task GetContentAsync_WithValidId_ShouldReturnAuthor()
    {
        // First get a valid ID from search
        var searchResult = await _service.SearchContentAsync(new Dictionary<string, string>());
        var validId = searchResult.FirstOrDefault()?.Id;
        
        Assert.IsNotNull(validId, "No authors found to test with");

        var result = await _service.GetContentAsync(validId);

        Assert.IsNotNull(result);
        Assert.AreEqual(validId, result.Id);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}
