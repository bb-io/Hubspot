using Apps.Common.Http;
using Apps.Hubspot.Dtos.Companies;
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
        public IEnumerable<CompanyDto> GetCompanies(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            var requestUrl = GetRequestUrl(url);
            return GetAll(requestUrl, null, authenticationCredentialsProvider).Select(CreateDtoByEntity).ToList();
        }

        [Action]
        public CompanyDto? GetCompany(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId
            )
        {
            var requestUrl = GetRequestUrl(url);
            var company = GetOne(requestUrl, companyId, null, authenticationCredentialsProvider);
            return company != null 
                ? CreateDtoByEntity(company) 
                : throw new InvalidOperationException($"Cannot get company: {companyId}");
        }

        [Action]
        public CompanyDto? CreateCompany(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] CreateOrUpdateCompanyDto dto
            )
        {
            var requestUrl = GetRequestUrl(url);
            var company = CreateDtoByEntity(dto);
            var createdCompany = Create(requestUrl, null, company, authenticationCredentialsProvider);
            return createdCompany != null
                ? CreateDtoByEntity(createdCompany)
                : null;
        }

        [Action]
        public CompanyDto? UpdateCompany(
            string url,
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId,
            [ActionParameter] CreateOrUpdateCompanyDto dto
            )
        {
            var requestUrl = GetRequestUrl(url);
            var company = CreateDtoByEntity(dto);
            var updatedCompany = Update(requestUrl, companyId, null, company, authenticationCredentialsProvider);
            return updatedCompany != null 
                ? CreateDtoByEntity(updatedCompany) 
                : throw new InvalidOperationException($"Cannot update company: {companyId}");
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

        private CompanyDto CreateDtoByEntity(Company company)
        {
            return new CompanyDto
            {
                Id = company.Id.ToString(),
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt,
                Archived = company.Archived,
                Domain = company.Properties.Domain,
                Name = company.Properties.Name,
                Createdate = company.Properties.Createdate,
                Hs_lastmodifieddate = company.Properties.Hs_lastmodifieddate,
                Hs_object_id = company.Properties.hs_object_id.ToString()
            };
        }

        private CreateOrUpdateCompany CreateDtoByEntity(CreateOrUpdateCompanyDto dto)
        {
            return new CreateOrUpdateCompany
            {
                Properties = new CreateOrUpdateCompanyProperties
                {
                    Domain = dto.Domain,
                    Name = dto.Name
                }
            };
        }
    }
}
