using Blackbird.Applications.Sdk.Common.Authentication;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Apps.Hubspot
{
    public class HubspotClient : RestClient
    {
        public HubspotClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) :
            base(new RestClientOptions() { BaseUrl = new Uri("https://api.hubapi.com") })
        { }

        public T? ExecuteWithError<T>(HubspotRequest request)
        {
            var res = this.Execute(request);
            if (!res.IsSuccessStatusCode)
            {
                var error = JsonConvert.DeserializeObject<Error>(res.Content);
                throw new Exception(error?.ToString());
            }
            return JsonConvert.DeserializeObject<T>(res.Content);
        }
    }
}
