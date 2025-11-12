using Apps.Hubspot.Actions.Content;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Requests.Emails;
using Blackbird.Filters.Coders;
using Newtonsoft.Json;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class BlacklakeTests : TestBase
{
    [TestMethod]
    public async Task Download_Blog_Has_Blacklake_required_Fields()
    {
        var contentId = "116907495204";
        var actions = new MetaActions(InvocationContext, FileManager);
        var result = await actions.DownloadContent(new()
        {
            ContentType = ContentTypes.Blog,
            ContentId = contentId
        });

        var contentString = FileManager.ReadOutputAsString(result.Content);
        var codedContent = (new HtmlContentCoder()).Deserialize(contentString, result.Content.Name);

        Console.WriteLine(contentString);
        Assert.AreEqual("en", codedContent.Language);
        Assert.AreEqual(contentId, codedContent.SystemReference.ContentId);
        Assert.AreEqual($"https://app.hubspot.com/blog/21491386/editor/{contentId}/content", codedContent.SystemReference.AdminUrl);
        Assert.AreEqual($"http://blackbird-21491386.hubspotpagebuilder.com/blog/starting-a-flight", codedContent.SystemReference.PublicUrl);
        Assert.AreEqual("Hubspot", codedContent.SystemReference.SystemName);
        Assert.AreEqual("https://www.hubspot.com", codedContent.SystemReference.SystemRef);
        Assert.IsNotNull(codedContent.SystemReference.ContentName);

        Assert.AreEqual("Hubspot", codedContent.Provenance.Review.Tool);
        Assert.AreEqual("https://www.hubspot.com", codedContent.Provenance.Review.ToolReference);

        Assert.IsTrue(codedContent.TextUnits.Any(x => x.Key is not null));
    }

    [TestMethod]
    public async Task Download_SitePage_Has_Blacklake_required_Fields()
    {
        var contentId = "116079994124";
        var actions = new MetaActions(InvocationContext, FileManager);
        var result = await actions.DownloadContent(new()
        {
            ContentType = ContentTypes.SitePage,
            ContentId = contentId
        });

        var contentString = FileManager.ReadOutputAsString(result.Content);
        var codedContent = (new HtmlContentCoder()).Deserialize(contentString, result.Content.Name);

        Console.WriteLine(contentString);
        Assert.AreEqual("en", codedContent.Language);
        Assert.AreEqual(contentId, codedContent.SystemReference.ContentId);
        Assert.AreEqual($"https://app.hubspot.com/pages/21491386/editor/{contentId}/content", codedContent.SystemReference.AdminUrl);
        Assert.AreEqual($"http://blackbird-21491386.hubspotpagebuilder.com/the-first-cup", codedContent.SystemReference.PublicUrl);
        Assert.AreEqual("Hubspot", codedContent.SystemReference.SystemName);
        Assert.AreEqual("https://www.hubspot.com", codedContent.SystemReference.SystemRef);
        Assert.IsNotNull(codedContent.SystemReference.ContentName);

        Assert.AreEqual("Hubspot", codedContent.Provenance.Review.Tool);
        Assert.AreEqual("https://www.hubspot.com", codedContent.Provenance.Review.ToolReference);

        Assert.IsTrue(codedContent.TextUnits.Any(x => x.Key is not null));
    }
}
