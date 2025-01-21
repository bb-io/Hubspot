using System.Net.Mime;
using System.Web;
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
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

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
        var html = HtmlConverter.ToHtml(email.Content, email.Name, email.Language, emailRequest.MarketingEmailId, ContentTypes.Email,email.BusinessUnitId);

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
        var htmlStream = await FileManagementClient.DownloadAsync(fileRequest.File);
        byte[] fileBytes;

        using (var memoryStream = new MemoryStream())
        {
            await htmlStream.CopyToAsync(memoryStream);
            fileBytes = memoryStream.ToArray();
        }

        var blackbirdId = HtmlConverter.ExtractBlackbirdId(fileBytes);
        var marketingEmailId = emailRequest.MarketingEmailId
         ?? blackbirdId
         ?? throw new PluginMisconfigurationException("Marketing email ID is required. Please provide it as an optional parameter or include it in the HTML file.");

        var titleText = HtmlConverter.ExtractTitle(fileBytes); 
        var language = HtmlConverter.ExtractLanguage(fileBytes);
        var businessUnitId = HtmlConverter.ExtractBusinessUnitId(fileBytes);

        using var stringStream = new MemoryStream(fileBytes);
        var (pageInfo, json) = HtmlConverter.ToJson(stringStream);
      
        var email = await GetEmail(marketingEmailId);

        var updatedContent = new Models.Requests.Emails.Content
        {
            FlexAreas = json["flexAreas"] as JObject,
            Widgets = json["widgets"] as JObject,
            StyleSettings = json["styleSettings"] as JObject,
            TemplatePath = json["templatePath"]?.ToString(),
            PlainTextVersion = json["plainTextVersion"]?.ToString() ?? ""
        };

        var updateRequest = new MarketingEmailOptionalRequest
        {
            Name = string.IsNullOrEmpty(emailRequest.Name)
                ? (string.IsNullOrEmpty(titleText) ? email.Name : titleText)
                : emailRequest.Name,
            Language = string.IsNullOrEmpty(emailRequest.Language)
                ? (string.IsNullOrEmpty(language) ? email.Language : language)
                : emailRequest.Language,
            BusinessUnitId = string.IsNullOrEmpty(emailRequest.BusinessUnitId)
                ? (string.IsNullOrEmpty(businessUnitId) ? email.BusinessUnitId : businessUnitId)
                : emailRequest.BusinessUnitId,
            Content = updatedContent
        };
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{marketingEmailId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds).WithJsonBody(updateRequest, JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Create marketing email from HTML", Description = "Create email from a HTML file content")]
    public async Task<MarketingEmailDto> CreateMarketingEmailFromHtml([ActionParameter] FileRequest fileRequest, [ActionParameter] CreateMarketingEmailOptionalRequest input)
    {
        var htmlFile = await FileManagementClient.DownloadAsync(fileRequest.File);
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(htmlFile);

        var title = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerHtml ?? "Default Title";
        var businessUnitId = htmlDoc.DocumentNode
            .SelectSingleNode("//meta[@name='business-unit-id']")
            ?.GetAttributeValue("content", null);
        var language = htmlDoc.DocumentNode
            .SelectSingleNode("/html/body")
            ?.GetAttributeValue("lang", "en");

        var originalContent = htmlDoc.DocumentNode
            .SelectSingleNode("/html/body")
            ?.GetAttributeValue("original", null);

        if (!string.IsNullOrEmpty(originalContent))
        {
            originalContent = HttpUtility.HtmlDecode(originalContent);
        }
        JObject contentJson = string.IsNullOrEmpty(originalContent) ? new JObject() : JObject.Parse(originalContent);

        var createRequest = new CreateMarketingEmailOptionalRequest
        {
            Name = input.Name ?? title,
            Language = input.Language ?? language ?? "en",
            BusinessUnitId = input.BusinessUnitId ?? businessUnitId ?? throw new PluginMisconfigurationException("Business Unit ID is required."),
            Content = new Models.Requests.Emails.Content
            {
                FlexAreas = contentJson["flexAreas"] as JObject,
                Widgets = contentJson["widgets"] as JObject,
                StyleSettings = contentJson["styleSettings"] as JObject,
                TemplatePath = contentJson["templatePath"]?.ToString(),
                PlainTextVersion = ""
            }
        };

        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Post, Creds)
        .WithJsonBody(createRequest, JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);
        return response;
    }

    private Task<EmailContentDto> GetEmail(string emailId)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{emailId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<EmailContentDto>(request);
    }
}

[ActionList] public class DebugActions(InvocationContext invocationContext) : BaseInvocable(invocationContext) { [Action("Debug", Description = "Debug action")] public List<AuthenticationCredentialsProvider> DebugAction() { return InvocationContext.AuthenticationCredentialsProviders.ToList(); } }