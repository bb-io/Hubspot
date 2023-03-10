using Apps.Hubspot.Dtos.Companies;
using Apps.Hubspot.Http;
using Apps.Hubspot.Models.Companies;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class CompanyActions : BaseActions<Company, CreateOrUpdateCompany>
    {
        private readonly string _requestUrl = "https://api.hubapi.com/crm/v3/objects/companies";

        public CompanyActions() : base(new HttpRequestProvider())
        {
        }

        [Action]
        public IEnumerable<CompanyDto> GetCompanies(
            AuthenticationCredentialsProvider authenticationCredentialsProvider
            )
        {
            return GetAll(_requestUrl, null, authenticationCredentialsProvider).Select(CreateDtoByEntity).ToList();
        }

        [Action]
        public CompanyDto? GetCompany(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId
            )
        {
            var company = GetOne(_requestUrl, companyId, null, authenticationCredentialsProvider);
            return company != null 
                ? CreateDtoByEntity(company) 
                : throw new InvalidOperationException($"Cannot get company: {companyId}");
        }

        [Action]
        public CompanyDto? CreateCompany(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] CreateOrUpdateCompanyDto dto
            )
        {
            var company = CreateDtoByEntity(dto);
            var createdCompany = Create(_requestUrl, null, company, authenticationCredentialsProvider);
            return createdCompany != null
                ? CreateDtoByEntity(createdCompany)
                : null;
        }

        [Action]
        public CompanyDto? UpdateCompany(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId,
            [ActionParameter] CreateOrUpdateCompanyDto dto
            )
        {
            var company = CreateDtoByEntity(dto);
            var updatedCompany = Update(_requestUrl, companyId, null, company, authenticationCredentialsProvider);
            return updatedCompany != null 
                ? CreateDtoByEntity(updatedCompany) 
                : throw new InvalidOperationException($"Cannot update company: {companyId}");
        }

        [Action]
        public void DeleteCompany(
            AuthenticationCredentialsProvider authenticationCredentialsProvider,
            [ActionParameter] long companyId
            )
        {
            Delete(_requestUrl, companyId, null, authenticationCredentialsProvider);
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
                    Name = dto.Name,
                    Phone = dto.Phone,
                    City = dto.City,
                    State = dto.State,
                    Industry  = dto.Industry,
                    
                }
            };
        }
    }
}
