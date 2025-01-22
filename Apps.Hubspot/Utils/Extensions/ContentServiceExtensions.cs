using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices;
using Apps.Hubspot.Services.ContentServices.Abstract;

namespace Apps.Hubspot.Utils.Extensions;

public static class ContentServiceExtensions
{
    public static async Task<List<Metadata>> ExecuteManyAsync(this List<IContentService> contentServices, 
        Dictionary<string, string> query)
    {
        var metadata = new List<Metadata>();
        foreach (var contentService in contentServices)
        {
            var result = await contentService.SearchContentAsync(query);
            metadata.AddRange(result);
        }

        return metadata;
    }
}