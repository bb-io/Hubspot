using Apps.Hubspot.Constants;
using Apps.Hubspot.Exceptions;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Hubspot.Api;

public class HubspotClient : BlackBirdRestClient
{
    public HubspotClient() : base(new()
    {
        BaseUrl = Urls.Api.ToUri()
    })
    {
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var error = JsonConvert.DeserializeObject<Error>(response.Content);
        return new HubspotException(error);
    }
}