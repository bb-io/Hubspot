using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.Forms;

public class CreateMarketingFormFromHtmlRequest
{
    [Display("Form name")]
    public string? Name { get; set; } = default!;

    [Display("Form type", Description = "Default value: hubspot"), StaticDataSource(typeof(FormTypeHandler))]
    public string? FormType { get; set; }

    public bool? Archived { get; set; }

    [StaticDataSource(typeof(LanguageHandler))]
    public string? Language { get; set; } = default!;

    public Dictionary<string, object> GetRequestBody()
    {
        var dictionary = new Dictionary<string, object>()
    {
        { "name", Name },
        { "formType", FormType ?? "hubspot" },
        { "createdAt", DateTime.UtcNow }
    };

        if (Archived.HasValue)
        {
            dictionary.Add("archived", Archived.Value);
        }

        if (Language != null)
        {
            dictionary.Add("configuration", new Dictionary<string, object>()
        {
            { "language", Language }
        });
        }

        return dictionary;
    }
}
