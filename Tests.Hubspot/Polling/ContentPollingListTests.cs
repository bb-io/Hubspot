using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Webhooks;
using Apps.Hubspot.Webhooks.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using Newtonsoft.Json;
using Tests.Hubspot.Base;

namespace Tests.Hubspot.Polling;

[TestClass]
public class ContentPollingListTests : TestBase
{
    private PollingList _pollingList;

    [TestInitialize]
    public void Initialize()
    {
        _pollingList = new PollingList(InvocationContext);
    }

    [TestMethod]
    public async Task OnContentCreatedOrUpdated_WithNullMemory_ShouldReturnCorrectResponse()
    {
        // Arrange
        var request = new PollingEventRequest<PageMemory>
        {
            Memory = null,
            PollingTime = DateTime.UtcNow
        };
        var languageRequest = new LanguageRequest { Language = "en" };
        var contentTypesFilter = new ContentTypesOptionalFilter();

        // Act
        var response = await _pollingList.OnContentCreatedOrUpdated(request, languageRequest, contentTypesFilter);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Memory.LastPollingTime);
        Assert.IsFalse(response.FlyBird);
        Assert.IsNull(response.Result);

        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"LastPollingTime: {response.Memory.LastPollingTime}");
    }

    [TestMethod]
    public async Task OnContentCreatedOrUpdated_WithExistingMemory_ShouldIdentifyContent()
    {
        // Arrange
        var memory = new PageMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-24) // Look back 24 hours
        };
        
        var request = new PollingEventRequest<PageMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var languageRequest = new LanguageRequest { Language = "en" };
        var contentTypesFilter = new ContentTypesOptionalFilter();

        // Act
        var response = await _pollingList.OnContentCreatedOrUpdated(request, languageRequest, contentTypesFilter);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        Assert.IsNotNull(response.Memory.LastPollingTime);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        Console.WriteLine($"LastPollingTime: {response.Memory.LastPollingTime}");
        
        if (response.FlyBird)
        {
            Assert.IsNotNull(response.Result);
            Assert.IsNotNull(response.Result.Metadata);
            Assert.IsTrue(response.Result.Metadata.Any());
            
            Console.WriteLine($"Found content items: {response.Result.Metadata.Count}");
            foreach (var item in response.Result.Metadata.Take(5)) // Log first 5 items
            {
                Console.WriteLine($"Item: {item.ContentId}, Type: {item.Type}, Title: {item.Title}, Updated: {item.UpdatedAt}");
            }
        }
    }

    [TestMethod]
    public async Task OnContentCreatedOrUpdated_WithBlogFilter_ShouldOnlyReturnBlogs()
    {
        // Arrange
        var memory = new PageMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-24) // Look back 24 hours
        };
        
        var request = new PollingEventRequest<PageMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var languageRequest = new LanguageRequest { Language = "en" };
        var contentTypesFilter = new ContentTypesOptionalFilter
        {
            ContentTypes = new List<string> { ContentTypes.Blog }
        };

        // Act
        var response = await _pollingList.OnContentCreatedOrUpdated(request, languageRequest, contentTypesFilter);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        
        if (response.FlyBird)
        {
            Assert.IsNotNull(response.Result);
            Assert.IsNotNull(response.Result.Metadata);
            Assert.IsTrue(response.Result.Metadata.All(m => m.Type == ContentTypes.Blog), 
                "All returned content should be blogs");
            
            Console.WriteLine($"Found blog items: {response.Result.Metadata.Count}");
            foreach (var item in response.Result.Metadata.Take(5))
            {
                Console.WriteLine($"Blog: {item.ContentId}, Title: {item.Title}, Updated: {item.UpdatedAt}");
            }
        }
    }

    [TestMethod]
    public async Task OnContentCreatedOrUpdated_WithPublishedFilter_ShouldOnlyReturnPublishedContent()
    {
        // Arrange
        var memory = new PageMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-24) // Look back 24 hours
        };
        
        var request = new PollingEventRequest<PageMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var languageRequest = new LanguageRequest { Language = "en" };
        var contentTypesFilter = new ContentTypesOptionalFilter
        {
            Published = true
        };

        // Act
        var response = await _pollingList.OnContentCreatedOrUpdated(request, languageRequest, contentTypesFilter);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        
        if (response.FlyBird)
        {
            Assert.IsNotNull(response.Result);
            Assert.IsNotNull(response.Result.Metadata);
            Assert.IsTrue(response.Result.Metadata.All(m => m.Published), 
                "All returned content should be published");
            
            Console.WriteLine($"Found published items: {response.Result.Metadata.Count}");
            foreach (var item in response.Result.Metadata.Take(5))
            {
                Console.WriteLine($"Published Item: {item.ContentId}, Type: {item.Type}, Title: {item.Title}");
            }
        }
    }

    [TestMethod]
    public async Task OnContentCreatedOrUpdated_WithMultipleContentTypesFilter_ShouldReturnFilteredContent()
    {
        // Arrange
        var memory = new PageMemory
        {
            LastPollingTime = DateTime.UtcNow.AddHours(-24) // Look back 24 hours
        };
        
        var request = new PollingEventRequest<PageMemory>
        {
            Memory = memory,
            PollingTime = DateTime.UtcNow
        };
        var languageRequest = new LanguageRequest { Language = "en" };
        var contentTypesFilter = new ContentTypesOptionalFilter
        {
            ContentTypes = new List<string> { ContentTypes.Blog, ContentTypes.Email }
        };

        // Act
        var response = await _pollingList.OnContentCreatedOrUpdated(request, languageRequest, contentTypesFilter);

        // Assert
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Memory);
        
        // Log for debugging
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
        
        if (response.FlyBird)
        {
            Assert.IsNotNull(response.Result);
            Assert.IsNotNull(response.Result.Metadata);
            Assert.IsTrue(response.Result.Metadata.All(m => 
                m.Type == ContentTypes.Blog || m.Type == ContentTypes.Email), 
                "All returned content should be blogs or emails");
            
            Console.WriteLine($"Found filtered items: {response.Result.Metadata.Count}");
            foreach (var item in response.Result.Metadata.Take(5))
            {
                Console.WriteLine($"Filtered Item: {item.ContentId}, Type: {item.Type}, Title: {item.Title}");
            }
        }
    }
}
