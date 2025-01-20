using Apps.Hubspot.Actions.Content;
using Apps.Hubspot.Constants;
using Newtonsoft.Json;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class MetaActionsTests : TestBase
{
    [TestMethod]
    public async Task SearchContent_WithoutFilters_ShouldNotFail()
    {
        var actions = new MetaActions(InvocationContext);

        var result = await actions.SearchContent(new()
        {
            ContentTypes = new[] { ContentTypes.Blog, ContentTypes.Email, ContentTypes.Form }
        }, new(), new());

        Assert.AreEqual(result.Items.Any(), true);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}