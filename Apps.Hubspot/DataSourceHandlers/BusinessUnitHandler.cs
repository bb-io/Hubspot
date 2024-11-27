using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Invocables;
using Apps.Hubspot.Models.Dtos.Business_units;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Hubspot.DataSourceHandlers
{
    public class BusinessUnitHandler : HubSpotInvocable, IAsyncDataSourceItemHandler
    {
        public BusinessUnitHandler(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var userId = await GetUserId();
            var endpoint = $"{Urls.BusinessUnits}/{userId}";
            var request = new HubspotRequest(endpoint, RestSharp.Method.Get, Creds);

            var response = await Client.ExecuteWithErrorHandling<GetAllResponse<BusinessUnitDto>>(request);

            return response.Results.Select(bu => new DataSourceItem
            {
                Value = bu.BusinessUnitId,
                DisplayName = bu.Name
            });
        }
    }
}
