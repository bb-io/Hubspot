using Apps.Hubspot.Actions;
using Newtonsoft.Json;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class PageActionsTests : TestBase
{
    [TestMethod]
    public async Task GetAllSitePages_WithCurrentStateInput_ShouldNotFail()
    {
        var actions = new PageActions(InvocationContext, FileManager);

        var result = await actions.GetAllSitePages(new()
        {
            CurrentState = "PUBLISHED"
        });

        Assert.AreEqual(result.Items.Any(), true);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }
}