using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Emails;
using Apps.Hubspot.Models.Requests.Emails;
using Apps.Hubspot.Models.Requests.Files;
using Apps.Hubspot.Models.Responses.Emails;
using Apps.Hubspot.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Hubspot.Actions;

[ActionList]
public class MarketingEmailsActions : BaseActions
{
    private readonly IFileManagementClient _fileManagementClient;

    public MarketingEmailsActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) :
        base(invocationContext, fileManagementClient)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Search marketing emails", Description = "Search for marketing emails based on provided filters")]
    public async Task<SearchMarketingEmailsResponse> SearchMarketingEmails([ActionParameter] SearchEmailsRequest input)
    {
        var query = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(input));
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}".WithQuery(query.AllIsNotNull());
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var result = await Client.Paginate<MarketingEmailDto>(request);
        return new(result);
    }

    [Action("Create marketing email", Description = "Create a new marketing email")]
    public Task<MarketingEmailDto> CreateMarketingEmail()
    {
        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Post, Creds);
        return Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);
    }

    [Action("Get marketing email content as HTML",
        Description = "Get content of a specific marketing email in HTML format")]
    public async Task<FileResponse> GetMarketingEmailHtml([ActionParameter] MarketingEmailRequest emailRequest)
    {
        var email = await GetEmail(emailRequest.MarketingEmailId);
        var html = EmailHtmlConverter.ToHtml((email["content"] as JObject)!);

        var file = await _fileManagementClient.UploadAsync(new MemoryStream(html), MediaTypeNames.Text.Html,
            $"{emailRequest}.html");

        return new()
        {
            File = file
        };
    }

    [Action("Update marketing email content from HTML",
        Description = "Update content of a specific marketing email from HTML file")]
    public async Task UpdateMarketingEmail([ActionParameter] MarketingEmailRequest emailRequest,
        [ActionParameter] FileRequest fileRequest)
    {
        var email = await GetEmail(emailRequest.MarketingEmailId);
        var htmlFile = await _fileManagementClient.DownloadAsync(fileRequest.File);

        email["content"] = EmailHtmlConverter.ToJson(htmlFile);

        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{emailRequest.MarketingEmailId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds).WithJsonBody(email);

        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<JObject> GetEmail(string emailId)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{emailId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<JObject>(request);
    }
}