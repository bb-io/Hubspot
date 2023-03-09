using Apps.Common.Http;
using Apps.Hubspot.Dtos.Companies;
using Apps.Hubspot.Dtos.Deals;
using Apps.Hubspot.Models.Contacts;
using Apps.Hubspot.Models.Deals;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class DealActions : BaseActions<Deal, CreateOrUpdateDeal>
    {
        private readonly string _requestUrl = "https://api.hubapi.com/crm/v3/objects/deals";
        public DealActions() : base(new HttpRequestProvider())
        {
        }

        [Action]
        public IEnumerable<DealDto> GetDeals(
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            return GetAll(_requestUrl, null, authenticationCredentialsProvider).Select(CreateDtoByEntity).ToList();
        }

        [Action]
        public DealDto? GetDeal(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long dealId
            )
        {
            var deal = GetOne(_requestUrl, dealId, null, authenticationCredentialsProvider);
            return deal != null
                ? CreateDtoByEntity(deal)
                : throw new InvalidOperationException($"Cannot get company: {dealId}");
        }

        [Action]
        public DealDto? CreateDeal(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] CreateOrUpdateDealDto dto
            )
        {
            var deal = CreateDtoByEntity(dto);
            var createdDeal = Create(_requestUrl, null, deal, authenticationCredentialsProvider);
            return createdDeal != null
                ? CreateDtoByEntity(createdDeal)
                : null;
        }

        [Action]
        public DealDto? UpdateDeal(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long dealId,
            [ActionParameter] CreateOrUpdateDealDto dto
            )
        {
            var deal = CreateDtoByEntity(dto);
            var updatedDeal = Update(_requestUrl, dealId, null, deal, authenticationCredentialsProvider);
            return updatedDeal != null
                ? CreateDtoByEntity(updatedDeal)
                : throw new InvalidOperationException($"Cannot update company: {dealId}");
        }

        [Action]
        public void DeleteDeal(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long dealId
            )
        {
            Delete(_requestUrl, dealId, null, authenticationCredentialsProvider);
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
