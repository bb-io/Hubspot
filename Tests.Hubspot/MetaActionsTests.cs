using Apps.Hubspot.Actions.Content;
using Apps.Hubspot.Constants;
using Newtonsoft.Json;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class MetaActionsTests : TestBase
{
    [TestMethod]
    public async Task SearchContent_WithAllContentTypes_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);

        var result = await actions.SearchContent(new()
        {
            ContentTypes = new[] { ContentTypes.Blog, ContentTypes.Email, ContentTypes.Form, ContentTypes.LandingPage, ContentTypes.SitePage }
        }, new(), new());

        Assert.AreEqual(result.Items.Any(), true);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task SearchContent_WithLandingPageContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);

        var result = await actions.SearchContent(new()
        {
            ContentTypes = new[] { ContentTypes.LandingPage }
        }, new(), new());

        Assert.AreEqual(result.Items.Any(), true);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task DownloadContent_WithBlogContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        var result = await actions.DownloadContent(new()
        {
            ContentType = ContentTypes.Blog,
            ContentId = "114373256889"
        });

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task DownloadContent_WithLandingPageContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        var result = await actions.DownloadContent(new()
        {
            ContentType = ContentTypes.LandingPage,
            ContentId = "68920460724"
        });

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task UpdateContent_WithLandingPageContentType_ShouldChangeName()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        var originalContent = await actions.GetContent(new()
        {
            ContentType = ContentTypes.LandingPage,
            ContentId = "68920460724"
        });
        
        var updatedContent = await actions.UpdateContent(new()
        {
            ContentType = ContentTypes.LandingPage,
            ContentId = "68920460724"
        }, new()
        {
            Title = $"{originalContent.Title}-updated"
        });
        
        Assert.AreNotEqual(originalContent.Title, updatedContent.Title);
        Assert.IsTrue(updatedContent.Title.EndsWith("-updated"));
        
        Console.WriteLine(JsonConvert.SerializeObject(updatedContent, Formatting.Indented));
        
        await actions.UpdateContent(new()
        {
            ContentType = ContentTypes.LandingPage,
            ContentId = "68920460724"
        }, new()
        {
            Title = originalContent.Title
        });
    }
    
    [TestMethod]
    public async Task UpdateContentFromHtml_WithBlogPostContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        await actions.UpdateContentFromHtml(new()
        {
            File = new()
            {
                Name = "Random sentences.html",
                ContentType = "text/html"
            },
            TargetLanguage = "de"
        });
    }
}