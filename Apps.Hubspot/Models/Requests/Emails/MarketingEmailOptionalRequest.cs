using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.Utils.Converters;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Requests.Emails;

public class MarketingEmailOptionalRequest
{
    [Display("Marketing email ID")]
    [DataSource(typeof(MarketingEmailDataHandler))]
    public string? MarketingEmailId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("language")]
    [StaticDataSource(typeof(LanguageHandler))]
    public string? Language { get; set; }

    [JsonProperty("businessUnitId")]
    [Display("Business unit ID", Description = "Business unit ID")]
    [DataSource(typeof(BusinessUnitHandler))]
    public string? BusinessUnitId { get; set; }

    [JsonProperty("content")]
    public Content? Content { get; set; }
}