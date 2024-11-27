using Apps.Hubspot.Constants;
using Apps.Hubspot.Exceptions;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
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

    public async Task<List<T>> Paginate<T>(RestRequest request)
    {
        var baseUrl = request.Resource;
        var after = string.Empty;

        var result = new List<T>();
        GetAllResponse<T> response;
        do
        {
            if (!string.IsNullOrEmpty(after))
                request.Resource = baseUrl.SetQueryParameter("after", after);

            response = await ExecuteWithErrorHandling<GetAllResponse<T>>(request);
            result.AddRange(response.Results);

            after = response.Paging?.Next?.After;
        } while (response.Total > result.Count);

        return result;
    }

}