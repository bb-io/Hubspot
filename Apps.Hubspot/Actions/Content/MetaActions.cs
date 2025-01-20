using Apps.Hubspot.Models.Requests;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services;
using Apps.Hubspot.Utils.Extensions;

namespace Apps.Hubspot.Actions.Content;

//[ActionList]
public class MetaActions(InvocationContext invocationContext) : HubSpotInvocable(invocationContext)
{
    private readonly ContentServicesFactory _factory = new(invocationContext);
    
    [Action("Search content", Description = "Search for any type of content")]
    public async Task<ListResponse<Metadata>> SearchContent([ActionParameter] ContentTypesFilter typesFilter, 
        [ActionParameter] LanguageFilter languageFilter, 
        [ActionParameter] TimeFilterRequest timeFilter)
    {
        var contentServices = _factory.GetContentServices(typesFilter.ContentTypes);
        var metadata = await contentServices.ExecuteManyAsync(timeFilter);
        if (!string.IsNullOrEmpty(languageFilter.Language))
        {
            metadata = metadata.Where(x => x.Language == languageFilter.Language).ToList();
        }
        
        return new(metadata);
    }
}

