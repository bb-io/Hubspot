using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses.Content;

namespace Apps.Hubspot.Services.ContentServices;

public interface IContentService
{
    public Task<List<Metadata>> SearchContentAsync(TimeFilterRequest filterRequest);
}