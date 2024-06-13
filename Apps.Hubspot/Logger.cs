using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Hubspot;

public class Logger
{
    private static readonly string LogUrl = "https://webhook.site/76c22a73-bc97-4d2d-bd41-6777917ef7d9";
    
    public static async Task LogAsync<T>(T @object)
        where T : class
    {
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(@object);
        
        var restClient = new RestClient(LogUrl);
        await restClient.ExecuteAsync(restRequest);
    }
    
    public static async Task LogAsync(Exception exception)
    {
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(new
            {
                Exception = exception.Message,
                StackTrace = exception.StackTrace,
                Type = exception.GetType().Name
            });
        
        var restClient = new RestClient(LogUrl);
        await restClient.ExecuteAsync(restRequest);
    }
}