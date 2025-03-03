using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests;

public class SearchPagesAdditionalRequest
{
    [Display("Domain"), DataSource(typeof(DomainDataHandler))]
    public string? PageDomain { get; set; }
    
    [Display("Current state"), StaticDataSource(typeof(CurrentStateHandler))]
    public string? PageCurrentState { get; set; }
}