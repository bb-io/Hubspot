using Apps.Hubspot.DataSourceHandlers;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Hubspot.Constants;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class DataSourceTests : TestBase
{
    [TestMethod]
    public async Task Get_blogposts_data_ShouldNotNullCollection()
    {
        var dataSource = new BlogPostHandler(InvocationContext);
        var data = await dataSource.GetDataAsync(new(), default);

        data.Should().NotBeNull();

        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
    }
    
    [TestMethod]
    public async Task ContentDataHandler_GetLandingPages_ShouldNotNullCollection()
    {
        var dataSource = new ContentDataHandler(InvocationContext, new() { ContentType = ContentTypes.LandingPage});
        var data = await dataSource.GetDataAsync(new(), default);

        data.Should().NotBeNull();
        Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
    }
}

