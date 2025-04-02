using Apps.Hubspot.Actions;
using Apps.Hubspot.Models.Requests.Emails;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Exceptions;
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
}
