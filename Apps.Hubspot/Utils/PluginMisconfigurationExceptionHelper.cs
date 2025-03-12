using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Hubspot.Utils;

public static class PluginMisconfigurationExceptionHelper
{
    public static void ThrowIsNullOrEmpty(string parameter, string parameterName)
    {
        if (string.IsNullOrEmpty(parameter))
        {
            throw new PluginMisconfigurationException($"Parameter {parameterName} is null or empty. Please provide a non-empty value.");
        }
    }

    public static void ThrowIfNull(object parameter, string parameterName)
    {
        if (parameter == null)
        {
            throw new PluginMisconfigurationException($"Parameter {parameterName} is null");
        }
    }
}
