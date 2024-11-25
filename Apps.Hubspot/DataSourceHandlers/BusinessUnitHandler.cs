using Apps.Hubspot.Api;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Business_units;
using Apps.Hubspot.Models.Responses;
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
            var userId = ApplicationConstants.ClientId;

            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User ID is empty");
            }

            var endpoint = $"/business-units/v3/business-units/user/{userId}";
            var request = new HubspotRequest(endpoint, RestSharp.Method.Get, Creds)
                .AddQueryParameter("properties", "logoMetadata");

            var response = await Client.ExecuteWithErrorHandling<GetAllResponse<BusinessUnitDto>>(request);

            return response.Results
            .ToDictionary(bu => bu.BusinessUnitId, bu => bu.Name);
        }
    }
}
