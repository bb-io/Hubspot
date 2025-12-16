using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;

namespace Apps.Hubspot.Services.ContentServices.Abstract;

public interface IContentService
{
    public Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query, SearchContentRequest searchContentRequest);
    public Task<Metadata> GetContentAsync(string id);
    public Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id);
    public Task<Stream> DownloadContentAsync(string id, LocalizablePropertiesRequest properties);
    public Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest);
    public Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest);
    public Task DeleteContentAsync(string id);
}