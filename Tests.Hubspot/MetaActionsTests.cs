using Apps.Hubspot.Actions;
using Apps.Hubspot.Actions.Content;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Requests;
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
            ContentTypes = [ContentTypes.Blog]
        }, new(), new(), new());

        Assert.AreEqual(result.Items.Any(), true);
        System.Console.WriteLine(result.Items.Count());
    }
    
    [TestMethod]
    public async Task SearchContent_WithLandingPageContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);

        var result = await actions.SearchContent(new()
        {
            ContentTypes = new[] { ContentTypes.LandingPage }
        }, new(), new(), new());

        Assert.AreEqual(result.Items.Any(), true);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task SearchContent_WithEmailContentTypeAndBlackbirdDomain_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        var expectedDomain = "blackbird-21491386.hubspotpagebuilder.com";
        
        var result = await actions.SearchContent(new()
        {
            ContentTypes = new[] { ContentTypes.Email }
        }, new(), new(), new()
        {
            Domain = expectedDomain
        });

        Assert.AreEqual(result.Items.Any(), true);
        foreach (var item in result.Items)
        {
            Assert.IsTrue(item.Domain.Equals(expectedDomain));    
        }
        
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task GetTranslationLanguageCodes_WithSitePageContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        
        var result = await actions.GetTranslationLanguageCodes(new()
        {
            ContentType = ContentTypes.SitePage,
            ContentId = "185234897561"
        });

        Assert.IsFalse(string.IsNullOrEmpty(result.PrimaryLanguage));
        CollectionAssert.Contains(result.TranslationLanguageCodes, "de");
        
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
        }, new LocalizablePropertiesRequest { });

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task DownloadContent_WithLandingPageContentType_ShouldNotFail()
    {
        var actions = new PageActions(InvocationContext, FileManager);
        var result = await actions.GetSitePageAsHtml(new()
        {
            PageId = "187626693691"
        }, new()
        {
            PropertiesToInclude = new []{"tab_icon"}
        });

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task DownloadContent_WithSitePageContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        var result = await actions.DownloadContent(new()
        {
            ContentType = ContentTypes.SitePage,
            ContentId = "116079994124"
        }, new LocalizablePropertiesRequest { });

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
            Content = new()
            {
                Name = "the first cup (1).html",
                ContentType = "text/html"
            },
            Locale = "de"
        }, new()
        {
            EnableInternalLinkLocalization = true,
            PublishedSiteBaseUrl = "https://blackbird-21491386.hubspotpagebuilder.com"
        });
    }
    
    [TestMethod]
    public async Task UpdateContentFromHtml_WithMarketingFormContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        await actions.UpdateContentFromHtml(new()
        {
            Content = new()
            {
                Name = "Форма опитування n1.html",
                ContentType = "text/html"
            },
            Locale = "en"
        }, new()
        {
            CreateNew = true
        });
    }
    
    [TestMethod]
    public async Task UpdateContentFromHtml_WithMarketingEmailContentType_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext, FileManager);
        await actions.UpdateContentFromHtml(new()
        {
            Content = new()
            {
                Name = "Warhammer 40k.html",
                ContentType = "text/html"
            },
            Locale = "uk"
        }, new()
        {
            CreateNew = true
        });
    }
}