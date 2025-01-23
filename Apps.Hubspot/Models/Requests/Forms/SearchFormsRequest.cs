using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.Extensions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.Forms;

public class SearchFormsRequest
{
    [Display("Archived", Description = "Whether to return only results that have been archived.")]
    public bool? Archived { get; set; }

    [Display("Form types", Description = "The form types to be included in the results."), StaticDataSource(typeof(FormTypeHandler))]
    public IEnumerable<string>? FormTypes { get; set; }
    
    [StaticDataSource(typeof(LanguageHandler))]
    public string? Language { get; set; }
    
    public string BuildQuery()
    {
        var query = new List<KeyValuePair<string, string>>();

        if (Archived.HasValue)
        {
            query.Add(new KeyValuePair<string, string>("archived", Archived.Value.ToString().ToLower()));
        }

        if (FormTypes?.Any() == true)
        {
            foreach (var formType in FormTypes)
            {
                query.Add(new KeyValuePair<string, string>("formTypes", formType));
            }
        }

        return query.ToQueryString();
    }
}