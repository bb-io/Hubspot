using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot
{
    public class HubspotClient : RestClient
    {
        public HubspotClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) :
            base(new RestClientOptions() { ThrowOnAnyError = true, BaseUrl = new Uri("https://api.hubapi.com") })
        { }
    }
}
