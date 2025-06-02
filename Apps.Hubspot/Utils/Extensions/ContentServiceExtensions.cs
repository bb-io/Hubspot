using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;

namespace Apps.Hubspot.Utils.Extensions;

public static class ContentServiceExtensions
{
    public static async Task<List<Metadata>> ExecuteManyAsync(this List<IContentService> contentServices, 
        Dictionary<string, string> query, SearchContentRequest searchContentRequest)
    {
        var metadata = new List<Metadata>();
        foreach (var contentService in contentServices)
        {
            var result = await contentService.SearchContentAsync(query, searchContentRequest);
            metadata.AddRange(result);
        }

        return metadata;
    }
}