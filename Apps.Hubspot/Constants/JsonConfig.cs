using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Apps.Hubspot.Constants;

public static class JsonConfig
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
}