using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Newtonsoft.Json;

namespace Apps.Hubspot.Extensions;

public static class ObjectExtensions
{
    public static Dictionary<string, string> AsQuery(this object obj)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(obj))!.AllIsNotNull();
    }
}