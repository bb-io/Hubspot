using Apps.Hubspot.Actions;
using Apps.Hubspot.Models.Requests.SitePages;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Hubspot.Base;

namespace Tests.Hubspot
{
    [TestClass]
    public class TranslateTests : TestBase
    {
        [TestMethod]
        public async Task TranslateSitePageFromFile_ShouldNotBeNull()
        {
            var actions = new PageActions(InvocationContext, FileManager);
            var translatePageRequest = new TranslateSitePageFromFileRequest
            {
                File = new FileReference
                {
                    Name = "test.html"
                },              
                TargetLanguage = "es"
            };
            var response = await actions.TranslateSitePageFromFile(translatePageRequest);

            Assert.IsNotNull(response);
        }
    }
}
