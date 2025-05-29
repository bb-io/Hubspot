using Apps.Hubspot.Constants;
using Apps.Hubspot.Exceptions;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Hubspot.Api;

public class HubspotClient() : BlackBirdRestClient(new()
{
    BaseUrl = Urls.Api.ToUri()
})
{
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        if (response.ContentType != null && response.ContentType.Contains("text/html"))
        {
            if (response.ContentType != null && response.ContentType.Contains("text/html"))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.Content);

                var statusCodeNode = htmlDoc.DocumentNode.SelectSingleNode("//h2[contains(text(), 'HTTP ERROR')]");
                var statusCode = 0;
                if (statusCodeNode != null)
                {
                    var statusText = statusCodeNode.InnerText;
                    var parts = statusText.Split(' ');
                    if (parts.Length >= 3 && int.TryParse(parts[2], out int code))
                    {
                        statusCode = code;
                    }
                }

                var messageNode = htmlDoc.DocumentNode.SelectSingleNode("//p");
                var errorMessage = messageNode != null ? messageNode.InnerText : "Unknown error";

                return new PluginApplicationException($"Error {statusCode}: {errorMessage}");
            }
        }

        if (string.IsNullOrEmpty(response.Content))
        {
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new PluginApplicationException($"Request was failed with the status: {response.StatusCode}");
            }

            return new PluginApplicationException(response.ErrorMessage);
        }

        var error = JsonConvert.DeserializeObject<Error>(response.Content)!;
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
            {
                request.Resource = baseUrl.SetQueryParameter("after", after);
            }

            response = await ExecuteWithErrorHandling<GetAllResponse<T>>(request);
            result.AddRange(response.Results);

            after = response.Paging?.Next?.After;
        } while (response.Total > result.Count);

        return result;
    }
}