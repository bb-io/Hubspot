using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Extensions;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos;
using Apps.Hubspot.Models.Dtos.Emails;
using Apps.Hubspot.Models.Requests.Emails;
using Apps.Hubspot.Models.Requests.Files;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Hubspot.Actions;

[ActionList]
public class MarketingEmailsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseActions(invocationContext, fileManagementClient)
{
    [Action("Search marketing emails", Description = "Search for marketing emails based on provided filters")]
    public async Task<ListResponse<MarketingEmailDto>> SearchMarketingEmails([ActionParameter] SearchEmailsRequest input)
    {
        var query = input.AsQuery();
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}".WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var result = await Client.Paginate<MarketingEmailDto>(request);
        return new(result);
    }

    [Action("Create marketing email", Description = "Create a new marketing email")]
    public Task<MarketingEmailDto> CreateMarketingEmail([ActionParameter] CreateMarketingEmailRequest input)
    {
        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Post, Creds)
            .WithJsonBody(input, JsonConfig.Settings);

        if (!string.IsNullOrEmpty(input.BusinessUnitId))
        {
            request.AddQueryParameter("businessUnitId", input.BusinessUnitId);
        }

        return Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);
    }

    [Action("Get marketing email content as HTML",
        Description = "Get content of a specific marketing email in HTML format")]
    public async Task<FileResponse> GetMarketingEmailHtml([ActionParameter] MarketingEmailRequest emailRequest)
    {
        var email = await GetEmail(emailRequest.MarketingEmailId);
        var html = HtmlConverter.ToHtml(email.Content, email.Name, email.Language, emailRequest.MarketingEmailId);

        var file = await FileManagementClient.UploadAsync(new MemoryStream(html), MediaTypeNames.Text.Html,
            $"{emailRequest}.html");

        return new()
        {
            File = file
        };
    }

    [Action("Update marketing email content from HTML",
        Description = "Update content of a specific marketing email from HTML file")]
    public async Task UpdateMarketingEmail([ActionParameter] MarketingEmailOptionalRequest emailRequest,
        [ActionParameter] FileRequest fileRequest)
    {
        var htmlFile = await FileManagementClient.DownloadAsync(fileRequest.File);
        var (pageInfo, json) = HtmlConverter.ToJson(htmlFile);
        
        var marketingEmailId = emailRequest.MarketingEmailId ?? pageInfo.HtmlDocument.ExtractBlackbirdReferenceId() ??
                               throw new InvalidOperationException("Marketing email ID is required. Please provide it as optional parameter or in the HTML file.");
        var email = await GetEmail(marketingEmailId);
        email.Content = json;

        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{marketingEmailId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds).WithJsonBody(email, JsonConfig.Settings);

        await Client.ExecuteWithErrorHandling(request);

       
    }

    [Action("Create marketing email from content of HTML", Description ="Create email from a HTML file content")]
    public async Task<MarketingEmailDto> CreateMarketingEmailFromHtml([ActionParameter] FileRequest fileRequest, [ActionParameter ] CreateMarketingEmailOptionalRequest input)
    {
        var htmlFile = await FileManagementClient.DownloadAsync(fileRequest.File);
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(htmlFile);

        var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
        var fileTitle = titleNode?.InnerText.Trim() ?? throw new PluginApplicationException("The HTML file does not contain a valid title.");

        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");
        if (bodyNode == null)
        {
            throw new PluginApplicationException("The HTML file does not contain a valid body section.");
        }

        var nameNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='name']");
        var htmlName = nameNode?.InnerText.Trim();

        var subjectNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='subject']");
        var htmlSubject = subjectNode?.InnerText.Trim();
        
        var name = input.Name?? htmlName?? fileTitle;
        var subject = input.Subject ?? htmlSubject ?? "Default Subject";

        var createRequest = new CreateMarketingEmailOptionalRequest
        {
            Name = name,
            Subject = subject,
            SendOnPublish = input.SendOnPublish,
            Archived = input.Archived,
            ActiveDomain = input.ActiveDomain,
            Language = input.Language,
            PublishDate = input.PublishDate,
            BusinessUnitId = input.BusinessUnitId
        };

        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Post, Creds)
       .WithJsonBody(createRequest, JsonConfig.Settings);

        return await Client.ExecuteWithErrorHandling<MarketingEmailDto>(request); 
    }

    private Task<EmailContentDto> GetEmail(string emailId)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{emailId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<EmailContentDto>(request);
    }
}

[ActionList] public class DebugActions(InvocationContext invocationContext) : BaseInvocable(invocationContext) { [Action("Debug", Description = "Debug action")] public List<AuthenticationCredentialsProvider> DebugAction() { return InvocationContext.AuthenticationCredentialsProviders.ToList(); } }