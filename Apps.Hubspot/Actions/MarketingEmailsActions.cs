using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Models.Dtos.Emails;
using Apps.Hubspot.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Hubspot.Actions;

[ActionList]
public class MarketingEmailsActions : BaseActions
{
    protected MarketingEmailsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
    }
    
    [Action("Search marketing emails", Description = "Search for marketing emails based on provided filters")]
    public Task<GetAllResponse<MarketingEmailDto>> SearchMarketingEmails()
    {
        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<GetAllResponse<MarketingEmailDto>>(request);
    }    
    
    [Action("Create marketing email", Description = "Create a new marketing email")]
    public Task<MarketingEmailDto> CreateMarketingEmail()
    {
        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Post, Creds);
        return Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);
    }    
    
    [Action("Update marketing email", Description = "Update a specific marketing email")]
    public Task<MarketingEmailDto> UpdateMarketingEmail()
    {
        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Patch, Creds);
        return Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);
    }
}