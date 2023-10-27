using Apps.Hubspot.Models.Responses;

namespace Apps.Hubspot.Exceptions;

public class HubspotException : Exception
{
    public string ErrorType { get; }
    public HubspotException(Error error) : base(error.Message)
    {
        ErrorType = error.ErrorType;
    }
}