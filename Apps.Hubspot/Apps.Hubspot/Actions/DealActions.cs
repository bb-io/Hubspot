using Apps.Hubspot.Http;
using Apps.Hubspot.Dtos.Deals;
using Apps.Hubspot.Models.Deals;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Actions;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class DealActions : BaseActions<Deal, CreateOrUpdateDeal>
    {
        private readonly string _requestUrl = "https://api.hubapi.com/crm/v3/objects/deals";
        public DealActions() : base(new HttpRequestProvider())
        {
        }

        [Action("Get all deals", Description = "Get a list of all the deals")]
        public IEnumerable<DealDto> GetDeals(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders
            )
        {
            return GetAll(_requestUrl, null, authenticationCredentialsProviders).Select(CreateDtoByEntity).ToList();
        }

        [Action("Get deal", Description = "Get information of a specific deal")]
        public DealDto? GetDeal(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long dealId
            )
        {
            var deal = GetOne(_requestUrl, dealId, null, authenticationCredentialsProviders);
            return deal != null
                ? CreateDtoByEntity(deal)
                : throw new InvalidOperationException($"Cannot get company: {dealId}");
        }

        [Action("Create deal", Description = "Create a new deal")]
        public DealDto? CreateDeal(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateOrUpdateDealDto dto
            )
        {
            var deal = CreateDtoByEntity(dto);
            var createdDeal = Create(_requestUrl, null, deal, authenticationCredentialsProviders);
            return createdDeal != null
                ? CreateDtoByEntity(createdDeal)
                : null;
        }

        [Action("Update deal", Description = "Update a deal's information")]
        public DealDto? UpdateDeal(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long dealId,
            [ActionParameter] CreateOrUpdateDealDto dto
            )
        {
            var deal = CreateDtoByEntity(dto);
            var updatedDeal = Update(_requestUrl, dealId, null, deal, authenticationCredentialsProviders);
            return updatedDeal != null
                ? CreateDtoByEntity(updatedDeal)
                : throw new InvalidOperationException($"Cannot update company: {dealId}");
        }

        [Action("Delete deal", Description = "Delete a deal")]
        public void DeleteDeal(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long dealId
            )
        {
            Delete(_requestUrl, dealId, null, authenticationCredentialsProviders);
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
