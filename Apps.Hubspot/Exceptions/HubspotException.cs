using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Hubspot.Exceptions;

public class HubspotException : PluginApplicationException
{
    public string ErrorType { get; }
    
    public HubspotException(Error error) : base(error.Message)
    {
        ErrorType = error.ErrorType;
    }
}