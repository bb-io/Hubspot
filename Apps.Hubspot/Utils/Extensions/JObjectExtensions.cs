using Blackbird.Applications.Sdk.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Utils.Extensions;

public static class JObjectExtensions
{
    public static JObject ToJObjectWithExceptionHandling(this string jsonString)
    {
        try
        {
            return JObject.Parse(jsonString);
        }
        catch (JsonReaderException ex)
        {
            throw new PluginApplicationException($"Failed to parse JSON string. The HTML file appears to be invalid or corrupted. Please verify the HTML file is correct. If you continue to experience issues, contact Blackbird support for assistance.", ex);
        }
    }
}
