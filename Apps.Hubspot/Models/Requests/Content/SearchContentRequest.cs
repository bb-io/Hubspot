using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests.Content;

public class SearchContentRequest
{
    [Display("Domain"), DataSource(typeof(DomainDataHandler))]
    public string? Domain { get; set; }

    [Display("Current state"), StaticDataSource(typeof(CurrentStateHandler))]
    public string? CurrentState { get; set; }
}