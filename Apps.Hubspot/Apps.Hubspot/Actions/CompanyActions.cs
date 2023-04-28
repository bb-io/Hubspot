using Apps.Hubspot.Dtos.Companies;
using Apps.Hubspot.Models;
using Apps.Hubspot.Models.Companies;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;
using System.ComponentModel.Design;

namespace Apps.Hubspot.Actions
{
    [ActionList]
    public class CompanyActions
    {

        [Action("Get all companies", Description = "Get a list of all companies")]
        public async Task<IEnumerable<CompanyDto>> GetCompaniesAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest("/crm/v3/objects/companies", Method.Get, authenticationCredentialsProviders);
            return client.Get<GetAllResponse<Company>>(request).Results.Select(c => CreateDtoByEntity(c)).ToList();
        }

        [Action("Get company", Description = "Get information of a specific company")]
        public CompanyDto? GetCompany(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long companyId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/companies/{companyId}", Method.Get, authenticationCredentialsProviders);
            return CreateDtoByEntity(client.Get<Company>(request));
        }

        [Action("Create company", Description = "Create a new company")]
        public CompanyDto? CreateCompany(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, 
            [ActionParameter] CreateOrUpdateCompanyDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/companies", Method.Post, authenticationCredentialsProviders);
            var company = CreateDtoByEntity(dto);
            request.AddJsonBody(company);
            return CreateDtoByEntity(client.Post<Company>(request));
        }

        [Action("Update company", Description = "Update a company's information")]
        public CompanyDto? UpdateCompany(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long companyId, [ActionParameter] CreateOrUpdateCompanyDto dto)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/companies", Method.Patch, authenticationCredentialsProviders);
            var company = CreateDtoByEntity(dto);
            request.AddJsonBody(company);
            return CreateDtoByEntity(client.Patch<Company>(request));
        }

        [Action("Delete company", Description = "Delete a company")]
        public void DeleteCompany(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] long companyId)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"/crm/v3/objects/companies/{companyId}", Method.Delete, authenticationCredentialsProviders);
            client.Execute(request);
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
