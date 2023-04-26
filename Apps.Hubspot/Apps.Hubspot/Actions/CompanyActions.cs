using Apps.Hubspot.Dtos.Companies;
using Apps.Hubspot.Http;
using Apps.Hubspot.Models.Companies;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
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

        [Action("Get all companies", Description = "Get a list of all companies")]
        public async Task<IEnumerable<CompanyDto>> GetCompaniesAsync(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders
            )
        {
            var companies = await GetAllAsync(_requestUrl, null, authenticationCredentialsProviders);
            return companies.Select(CreateDtoByEntity).ToList();
        }

        [Action("Get company", Description = "Get information of a specific company")]
        public CompanyDto? GetCompany(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long companyId
            )
        {
            var company = GetOne(_requestUrl, companyId, null, authenticationCredentialsProviders);
            return company != null 
                ? CreateDtoByEntity(company) 
                : throw new InvalidOperationException($"Cannot get company: {companyId}");
        }

        [Action("Create company", Description = "Create a new company")]
        public CompanyDto? CreateCompany(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] CreateOrUpdateCompanyDto dto
            )
        {
            var company = CreateDtoByEntity(dto);
            var createdCompany = Create(_requestUrl, null, company, authenticationCredentialsProviders);
            return createdCompany != null
                ? CreateDtoByEntity(createdCompany)
                : null;
        }

        [Action("Update company", Description = "Update a company's information")]
        public CompanyDto? UpdateCompany(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long companyId,
            [ActionParameter] CreateOrUpdateCompanyDto dto
            )
        {
            var company = CreateDtoByEntity(dto);
            var updatedCompany = Update(_requestUrl, companyId, null, company, authenticationCredentialsProviders);
            return updatedCompany != null 
                ? CreateDtoByEntity(updatedCompany) 
                : throw new InvalidOperationException($"Cannot update company: {companyId}");
        }

        [Action("Delete company", Description = "Delete a company")]
        public void DeleteCompany(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long companyId
            )
        {
            Delete(_requestUrl, companyId, null, authenticationCredentialsProviders);
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
