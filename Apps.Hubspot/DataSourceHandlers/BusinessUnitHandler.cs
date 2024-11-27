using Apps.Hubspot.Api;
using Apps.Hubspot.Auth.OAuth2;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Business_units;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Hubspot.DataSourceHandlers
{
    public class BusinessUnitHandler : HubSpotInvocable, IAsyncDataSourceHandler
    {
        public BusinessUnitHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var userId = await GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID is missing or invalid.");
            }

            var endpoint = $"{Urls.BusinessUnits}/{userId}";
            var request = new HubspotRequest(endpoint, RestSharp.Method.Get, Creds);

            var response = await Client.ExecuteWithErrorHandling<GetAllResponse<BusinessUnitDto>>(request);

            return response.Results
            .ToDictionary(bu => bu.BusinessUnitId, bu => bu.Name);
        }
    }
}
