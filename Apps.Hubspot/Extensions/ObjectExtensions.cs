using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Newtonsoft.Json;

namespace Apps.Hubspot.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, string> AsQuery(this object obj)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(obj))!.AllIsNotNull();
    }

    public static Dictionary<string, string> Combine(this Dictionary<string, string> first, params Dictionary<string, string>[] others)
    {
        var combined = new Dictionary<string, string>(first);

        foreach (var dictionary in others)
        {
            foreach (var kvp in dictionary)
            {
                if (!combined.ContainsKey(kvp.Key))
                {
                    combined.Add(kvp.Key, kvp.Value);
                }
            }
        }

        return combined;
    }

    public static string ToQueryString(this Dictionary<string, string> query)
    {
        return string.Join("&", query.Select(x => $"{x.Key}={x.Value}"));
    }

    public static string ToQueryString(this List<KeyValuePair<string, string>>? parameters)
    {
        if (parameters == null || !parameters.Any())
            return string.Empty;

        var array = parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}__eq={Uri.EscapeDataString(p.Value)}").ToArray();
        return "?" + string.Join("&", array);
    }

    public static Dictionary<string, string> AsHubspotQuery(this object obj)
    {
        var query = new Dictionary<string, string>();
        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            var jsonProperty = property.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                .FirstOrDefault() as JsonPropertyAttribute;

            var key = jsonProperty?.PropertyName ?? property.Name;
            var value = property.GetValue(obj);

            if (value == null)
                continue;

            switch (value)
            {
                case DateTime dateTime:
                    var unixTime = new DateTimeOffset(dateTime.ToUniversalTime())
                        .ToUnixTimeMilliseconds()
                        .ToString();
                    query[key] = unixTime;
                    break;

                case IEnumerable<string> stringList: query[key] = string.Join(",", stringList);
                    break;

                default:
                    if (value is System.Collections.IEnumerable enumerable && value is not string)
                    {
                        var list = enumerable.Cast<object>().Select(x => x?.ToString());
                        query[key] = string.Join(",", list);
                    }
                    else
                    {
                        query[key] = value.ToString();
                    }
                    break;
            }
        }

        return query;
    }
}