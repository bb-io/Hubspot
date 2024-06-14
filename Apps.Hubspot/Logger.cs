using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Hubspot;

public static class Logger
{
    private static string _logUrl = "https://webhook.site/76c22a73-bc97-4d2d-bd41-6777917ef7d9";
    
    public static void Log<T>(T obj)
        where T : class
    {
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        var restClient = new RestClient(_logUrl);
        
        restClient.Execute(restRequest);
    }

    public static async Task LogAsync<T>(T obj)
        where T : class
    {
        var restRequest = new RestRequest(string.Empty, Method.Post)
            .WithJsonBody(obj);
        var restClient = new RestClient(_logUrl);
        
        await restClient.ExecuteAsync(restRequest);
    }
    
    public static async Task LogExceptionAsync(Exception ex)
    {
        var obj = new
        {
            ExceptionMessage = ex.Message,
            ExceptionType = ex.GetType().Name,
            ex.StackTrace,
            InnerException = ex.InnerException?.Message
        };
        
        await LogAsync(obj);
    }
}