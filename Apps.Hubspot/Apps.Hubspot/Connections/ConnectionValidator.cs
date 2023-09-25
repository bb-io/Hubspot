using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Hubspot.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
        try
        {
            var client = new HubspotClient();
            var request = new HubspotRequest(ApiEndpoints.SitePages(), Method.Get, authProviders);
            await client.ExecuteWithErrorHandling(request);

            return new()
            {
                IsValid = true
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
    }
}