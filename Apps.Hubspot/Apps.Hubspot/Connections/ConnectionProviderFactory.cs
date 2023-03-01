using Apps.Hubspot.Bearer;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Connections
{
    public class ConnectionProviderFactory : IConnectionProviderFactory
    {
        public IEnumerable<IConnectionProvider> Create()
        {
            yield return new BearerConnectionProvider();
        }
    }
}
