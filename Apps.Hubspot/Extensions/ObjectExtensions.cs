using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Newtonsoft.Json;

namespace Apps.Hubspot.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, string> AsQuery(this object obj)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(obj))!.AllIsNotNull();
    }
    
    public static string ToQueryString(this Dictionary<string, string> query)
    {
        return string.Join("&", query.Select(x => $"{x.Key}__eq={x.Value}")); //TODO: fix this query, as it is it does not run.
    }
    
    public static string ToQueryString(this List<KeyValuePair<string, string>>? parameters)
    {
        if (parameters == null || !parameters.Any())
            return string.Empty;

        var array = parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}__eq={Uri.EscapeDataString(p.Value)}").ToArray();
        return "?" + string.Join("&", array);
    }
}