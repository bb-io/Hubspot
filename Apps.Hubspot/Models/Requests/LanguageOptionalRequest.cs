using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Hubspot.Models.Requests;

public class LanguageOptionalRequest
{
    [DataSource(typeof(LanguageHandler))]
    public string? Language { get; set; }
}