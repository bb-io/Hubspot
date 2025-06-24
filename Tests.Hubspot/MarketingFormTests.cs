using Apps.Hubspot.Actions;
using Apps.Hubspot.Models.Requests.Files;
using Apps.Hubspot.Models.Requests.Forms;
using Blackbird.Applications.Sdk.Common.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Hubspot.Base;

namespace Tests.Hubspot
{
    [TestClass]
    public class MarketingFormTests :TestBase
    {
        [TestMethod]
        public async Task CreateMarketingFormsTest()
        {
            var action = new MarketingFormActions(InvocationContext, FileManager);
            var request = new CreateMarketingFormFromHtmlRequest
            {
                Name = "Test Form",
            };

            var file = new FileRequest { File = new FileReference { Name = "Warhammer 40k.html" } };
            var response = await action.CreateMarketingFormFromHtml(file,request);
            Assert.IsNotNull(response);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}
