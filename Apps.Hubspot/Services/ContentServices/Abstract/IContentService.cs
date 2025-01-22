using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses.Content;

namespace Apps.Hubspot.Services.ContentServices.Abstract;

public interface IContentService
{
    public Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query);
    public Task<Metadata> GetContentAsync(string id);
    public Task<Stream> DownloadContentAsync(string id);
    public Task UpdateContentFromHtmlAsync(string targetLanguage, Stream stream);
    public Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest);
    public Task DeleteContentAsync(string id);
}