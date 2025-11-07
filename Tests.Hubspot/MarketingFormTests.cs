using Apps.Hubspot.Actions;
using Apps.Hubspot.Models.Requests.Files;
using Apps.Hubspot.Models.Requests.Forms;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class MarketingFormTests : TestBase
{
    [TestMethod]
    public async Task CreateMarketingFormsTest()
    {
        var action = new MarketingFormActions(InvocationContext, FileManager);
        var request = new CreateMarketingFormFromHtmlRequest
        {
            Name = "Test Form",
        };

        var file = new FileRequest { File = new FileReference { Name = "Форма опитування n1.html" } };
        var response = await action.CreateMarketingFormFromHtml(file, request);
        Assert.IsNotNull(response);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
    }

    [TestMethod]
    public async Task GetMarketingFormsTest()
    {
        var action = new MarketingFormActions(InvocationContext, FileManager);
        var request = new MarketingFormRequest
        {
            FormId= "c6ebcd7e-5974-45a4-8bdd-14d4467bece4"
        };


        var response = await action.GetMarketingFormAsHtml(request, false);
        Assert.IsNotNull(response);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
    }
}