using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests;

public class LanguageRequest
{
    [StaticDataSource(typeof(LanguageHandler))]
    public string? Language { get; set; }
}