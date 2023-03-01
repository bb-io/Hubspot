using Apps.Common.Http;
using Apps.Hubspot.Models.Companies;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class CompanyActions : BaseActions<Company, CreateOrUpdateCompany>
    {
        public CompanyActions() : base(new HttpRequestProvider())
        {
        }

        [Action]
        public IEnumerable<Company> GetCompanies(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetAll(requestUrl, null, authenticationCredentialsProvider);
        }

        [Action]
        public Company? GetCompany(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetOne(requestUrl, companyId, null, authenticationCredentialsProvider);
        }

        [Action]
        public Company? CreateCompany(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] CreateOrUpdateCompany company
            )
        {
            var requestUrl = GetRequestUrl(url);
            return Create(requestUrl, null, company, authenticationCredentialsProvider);
        }

        [Action]
        public Company? UpdateCompany(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId,
            [ActionParameter] CreateOrUpdateCompany company
            )
        {
            var requestUrl = GetRequestUrl(url);
            return Update(requestUrl, companyId, null, company, authenticationCredentialsProvider);
        }

        [Action]
        public void DeleteCompany(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId
            )
        {
            var requestUrl = GetRequestUrl(url);
            Delete(requestUrl, companyId, null, authenticationCredentialsProvider);
        }

        private string GetRequestUrl(string url)
        {
            const string requestUrlFormat = "{0}/crm/v3/objects/companies";
            return string.Format(requestUrlFormat, url);
        }
    }
}
