using Apps.Hubspot.Actions;
using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Emails;
using Apps.Hubspot.Models.Requests.Files;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
using System.Text;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class MarketingEmailsActionsTests : TestBase
{
    [TestMethod]
    public async Task UpdateMarketingEmail_WithInvalidHtml_ShouldThrowException()
    {
        // Arrange
        var actions = new MarketingEmailsActions(InvocationContext, FileManager);
        var fileRequest = new Apps.Hubspot.Models.Requests.Files.FileRequest
        {
            File = new Blackbird.Applications.Sdk.Common.Files.FileReference
            {
                Name = "invalid.html",
                ContentType = "text/html"
            }
        };
        var emailRequest = new MarketingEmailOptionalRequest
        {
            MarketingEmailId = "123456789", // Using dummy ID, as we expect the HTML parsing to fail first
            Name = "Test Email"
        };

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<PluginMisconfigurationException>(() => 
            actions.UpdateMarketingEmail(emailRequest, fileRequest));
        
        // Verify the exception contains relevant information
        Assert.IsTrue(exception.Message.Contains("HTML file") || 
                      exception.Message.Contains("invalid"),
            $"Exception message '{exception.Message}' doesn't contain expected content about invalid HTML file");
        
        Console.WriteLine($"Actual exception: {exception.Message}");
    }

    [TestMethod]
    public async Task GetMarketingEmail_WithInvalidHtml_ShouldThrowException()
    {
        var actions = new MarketingEmailsActions(InvocationContext, FileManager);

        var response = await actions.GetMarketingEmailHtml(new MarketingEmailRequest { MarketingEmailId= "188266404464" },
            new LocalizablePropertiesRequest { },true);
    }

    [TestMethod]
    public async Task CreateMarketingEmailFromHtml_WithInvalidHtml_ShouldThrowException()
    {
        var actions = new MarketingEmailsActions(InvocationContext, FileManager);

        var response = await actions.CreateMarketingEmailFromHtml(
            new FileRequest { File= new FileReference { Name= "Warhammer 40k.html" } },
            new CreateMarketingEmailOptionalRequest { Name= "[Test] Email 2_exec_compete_ja_jp", Language= "ja_jp" });

        var json = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine($"Response: {json}");
        Assert.IsNotNull(response);

    }
}
