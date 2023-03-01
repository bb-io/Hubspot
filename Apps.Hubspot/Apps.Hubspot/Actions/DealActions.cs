using Apps.Common.Http;
using Apps.Hubspot.Models.Deals;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class DealActions : BaseActions<Deal, CreateOrUpdateDeal>
    {
        public DealActions() : base(new HttpRequestProvider())
        {
        }

        [Action]
        public IEnumerable<Deal> GetDeals(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetAll(requestUrl, null, authenticationCredentialsProvider);
        }

        [Action]
        public Deal? GetDeal(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long dealId
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetOne(requestUrl, dealId, null, authenticationCredentialsProvider);
        }

        [Action]
        public Deal? CreateDeal(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] CreateOrUpdateDeal deal
            )
        {
            var requestUrl = GetRequestUrl(url);
            return Create(requestUrl, null, deal, authenticationCredentialsProvider);
        }

        [Action]
        public Deal? UpdateDeal(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long dealId,
            [ActionParameter] CreateOrUpdateDeal deal
            )
        {
            var requestUrl = GetRequestUrl(url);
            return Update(requestUrl, dealId, null, deal, authenticationCredentialsProvider);
        }

        [Action]
        public void DeleteDeal(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long dealId
            )
        {
            var requestUrl = GetRequestUrl(url);
            Delete(requestUrl, dealId, null, authenticationCredentialsProvider);
        }

        private string GetRequestUrl(string url)
        {
            const string requestUrlFormat = "{0}/crm/v3/objects/deals";
            return string.Format(requestUrlFormat, url);
        }
    }
}
