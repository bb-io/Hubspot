using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices;

namespace Apps.Hubspot.Utils.Extensions;

public static class ContentServiceExtensions
{
    public static async Task<List<Metadata>> ExecuteManyAsync(this List<IContentService> contentServices, 
        TimeFilterRequest filterRequest)
    {
        var metadata = new List<Metadata>();
        foreach (var contentService in contentServices)
        {
            var result = await contentService.SearchContentAsync(filterRequest);
            metadata.AddRange(result);
        }

        return metadata;
    }
}