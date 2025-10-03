using Apps.Hubspot.Auth.OAuth2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Hubspot.Base;

namespace Tests.Hubspot;

[TestClass]
public class OAuth2TokenServiceTests : TestBase
{
    [TestMethod]
    public async Task RequestToken_WithValidCodeAndState_ShouldReturnTokenDictionary()
    {
        // Arrange
        var tokenService = new OAuth2TokenService(InvocationContext);
        var state = "50bd3583-a4e3-4c95-a486-b605b3f2bdd2";
        var code = "na1-578b-4e32-44f1-b867-56a39e969ff5";
        var values = new Dictionary<string, string>();

        //https://bridge.blackbird.io/api/AuthorizationCode?code=na1-578b-4e32-44f1-b867-56a39e969ff5&state=5ad4b79f-44cc-4ee9-8b30-fbbb6412cec8
        var result = await tokenService.RequestToken(state, code, values, CancellationToken.None);

        // Verify the result contains expected keys
        Assert.IsNotNull(result);
        Assert.IsTrue(result.ContainsKey("access_token"));
        Assert.IsTrue(result.ContainsKey("expires_at"));
    }
}
