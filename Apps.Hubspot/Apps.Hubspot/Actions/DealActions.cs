using Apps.Hubspot.Dtos.Deals;
using Apps.Hubspot.Models.Deals;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Actions;
using Apps.Hubspot.Models.Contacts;
using Apps.Hubspot.Models;
using RestSharp;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class DealActions
    {

        [Action("Get all deals", Description = "Get a list of all the deals")]
        public IEnumerable<DealDto> GetDeals(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest("/crm/v3/objects/deals", Method.Get, authenticationCredentialsProviders);
            return client.Get<GetAllResponse<Deal>>(request).Results.Select(c => CreateDtoByEntity(c)).ToList();
        }

        [Action("Get deal", Description = "Get information of a specific deal")]
        public DealDto? GetDeal(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long dealId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/deals/{dealId}", Method.Get, authenticationCredentialsProviders);
            return CreateDtoByEntity(client.Get<Deal>(request));
        }

        [Action("Create deal", Description = "Create a new deal")]
        public DealDto? CreateDeal(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateOrUpdateDealDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/deals", Method.Post, authenticationCredentialsProviders);
            var deal = CreateDtoByEntity(dto);
            request.AddJsonBody(deal);
            return CreateDtoByEntity(client.Post<Deal>(request));
        }

        [Action("Update deal", Description = "Update a deal's information")]
        public DealDto? UpdateDeal(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long dealId,[ActionParameter] CreateOrUpdateDealDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/deals", Method.Patch, authenticationCredentialsProviders);
            var deal = CreateDtoByEntity(dto);
            request.AddJsonBody(deal);
            return CreateDtoByEntity(client.Patch<Deal>(request));
        }

        [Action("Delete deal", Description = "Delete a deal")]
        public void DeleteDeal(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long dealId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/deals/{dealId}", Method.Delete, authenticationCredentialsProviders);
            client.Execute(request);
        }

        private DealDto CreateDtoByEntity(Deal deal)
        {
            return new DealDto
            {
                Id = deal.Id,
                CreatedAt = deal.CreatedAt,
                UpdatedAt = deal.UpdatedAt,
                Archived = deal.Archived,
                Amount = deal.Properties.Amount,
                Dealname = deal.Properties.Dealname,
                Closedate = deal.Properties.Closedate,
                Dealstage= deal.Properties.Dealstage,
                Pipeline = deal.Properties.Pipeline,
                Createdate = deal.Properties.Createdate,
                Hs_lastmodifieddate = deal.Properties.Hs_lastmodifieddate,
                Hs_object_id = deal.Properties.Hs_object_id
            };
        }

        private CreateOrUpdateDeal CreateDtoByEntity(CreateOrUpdateDealDto dto)
        {
            return new CreateOrUpdateDeal
            {
                Properties = new CreateOrUpdateDealProperties
                {
                    Amount = dto.Amount,
                    Dealname = dto.Dealname,
                    Closedate = dto.Closedate,
                    Dealstage = dto.Dealstage,
                    Pipeline = dto.Pipeline,
                }
            };
        }
    }
}
