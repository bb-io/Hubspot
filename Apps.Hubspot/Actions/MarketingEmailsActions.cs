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
using Apps.Hubspot.Utils.Extensions;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
    public async Task<FileResponse> GetMarketingEmailHtml2([ActionParameter] MarketingEmailRequest emailRequest)
    {
        var email = await GetEmail(emailRequest.MarketingEmailId);

        var html = GenerateHtmlFromEmail(email);

        var file = await FileManagementClient.UploadAsync(
        new MemoryStream(Encoding.UTF8.GetBytes(html)),
        MediaTypeNames.Text.Html,
        $"{emailRequest.MarketingEmailId}.html");

        return new FileResponse
        {
            File = file
        };
    }


    private static string GenerateHtmlFromEmail(MarketingEmailContentDto email)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='en'>");
        sb.AppendLine("<head>");
        sb.AppendLine($"<title>{email.Name}</title>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        sb.AppendLine($"<div id='id'>{email.Id}</div>");
        sb.AppendLine($"<div id='name'>{email.Name}</div>");
        sb.AppendLine($"<div id='subject'>{email.Subject}</div>");
        sb.AppendLine($"<div id='sendOnPublish'>{email.SendOnPublish.ToString().ToLower()}</div>");
        sb.AppendLine($"<div id='archived'>{email.Archived.ToString().ToLower()}</div>");
        sb.AppendLine($"<div id='language'>{email.Language}</div>");
        sb.AppendLine($"<div id='activeDomain'>{email.ActiveDomain}</div>");
        sb.AppendLine($"<div id='publishDate'>{email.PublishDate:yyyy-MM-ddTHH:mm:ss}</div>");
        sb.AppendLine($"<div id='businessUnitId'>{email.BusinessUnitId}</div>");

        if (!string.IsNullOrWhiteSpace(email.HtmlVersion))
        {
            sb.AppendLine($"<div id='htmlContent'>{email.HtmlVersion}</div>");
        }

        if (email.Content != null)
        {
            sb.AppendLine("<div id='content'>");
            sb.AppendLine(email.Content.ToString(Newtonsoft.Json.Formatting.Indented)
                .Replace("<", "&lt;")
                .Replace(">", "&gt;"));
            sb.AppendLine("</div>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }


    [Action("Update marketing email content from HTML",
        Description = "Update content of a specific marketing email from HTML file")]
    public async Task UpdateMarketingEmail([ActionParameter] MarketingEmailOptionalRequest emailRequest,
        [ActionParameter] FileRequest fileRequest)
    {
        var htmlFile = await FileManagementClient.DownloadAsync(fileRequest.File);
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(htmlFile);

        var extractedValues = Apps.Hubspot.Utils.Extensions.HtmlExtensions.ExtractHtmlValuesForEmail(htmlDoc);

        var marketingEmailId = emailRequest.MarketingEmailId
                           ?? extractedValues.Id
                           ?? throw new PluginMisconfigurationException("Marketing email ID is required. Please provide it as optional parameter or in the HTML file.");

        var updateRequest = new MarketingEmailOptionalRequest
        {
            MarketingEmailId = marketingEmailId,
            Name = emailRequest.Name ?? extractedValues.Name ?? "Updated Default Name",
            Subject = emailRequest.Subject ?? extractedValues.Subject ?? "Updated Default Subject",
            SendOnPublish = emailRequest.SendOnPublish ?? extractedValues.SendOnPublish ?? false,
            Archived = emailRequest.Archived ?? extractedValues.Archived ?? false,
            ActiveDomain = emailRequest.ActiveDomain ?? extractedValues.ActiveDomain ?? null,
            Language = emailRequest.Language ?? extractedValues.Language ?? "en",
            PublishDate = emailRequest.PublishDate ?? extractedValues.PublishDate ?? DateTime.UtcNow,
            BusinessUnitId = emailRequest.BusinessUnitId ?? extractedValues.BusinessUnitId
                          ?? throw new PluginMisconfigurationException("Please enter the business ID or add it in the HTML file"),
            Content = string.IsNullOrWhiteSpace(extractedValues.Body)? null : new Content { HtmlVersion = extractedValues.Body }
        };

        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{marketingEmailId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds).WithJsonBody(updateRequest, JsonConfig.Settings);
        var response = await Client.ExecuteAsync(request);
    }

    [Action("Create marketing email from HTML", Description ="Create email from a HTML file content")]
    public async Task<MarketingEmailDto> CreateMarketingEmailFromHtml([ActionParameter] FileRequest fileRequest, [ActionParameter ] CreateMarketingEmailOptionalRequest input)
    {
        var htmlFile = await FileManagementClient.DownloadAsync(fileRequest.File);
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(htmlFile);

        var extractedValues = Apps.Hubspot.Utils.Extensions.HtmlExtensions.ExtractHtmlValuesForEmail(htmlDoc);

        var createRequest = new CreateMarketingEmailOptionalRequest
        {
            Name = input.Name ?? extractedValues.Name,
            Subject = input.Subject ?? extractedValues.Subject ?? "Default Subject",
            SendOnPublish = input.SendOnPublish ?? extractedValues.SendOnPublish ?? false,
            Archived = input.Archived ?? extractedValues.Archived ?? false,
            ActiveDomain = input.ActiveDomain ?? extractedValues.ActiveDomain ?? null,
            Language = input.Language ?? extractedValues.Language ?? "en",
            PublishDate = input.PublishDate ?? extractedValues.PublishDate,
            BusinessUnitId = input.BusinessUnitId ?? extractedValues.BusinessUnitId
                         ?? throw new PluginMisconfigurationException("Please enter the business ID or add it in the HTML file"),
            Content = string.IsNullOrWhiteSpace(extractedValues.Body) ? null : new Content { HtmlVersion = extractedValues.Body }
        };

        var request = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Post, Creds)
       .WithJsonBody(createRequest, JsonConfig.Settings);

        return await Client.ExecuteWithErrorHandling<MarketingEmailDto>(request); 
    }

    private async Task<MarketingEmailContentDto> GetEmail(string emailId)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{emailId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<JObject>(request);


        return new MarketingEmailContentDto
        {
            Id = response["id"]?.ToString(),
            Name = response["name"]?.ToString(),
            Subject = response["subject"]?.ToString(),
            SendOnPublish = response["sendOnPublish"]?.ToObject<bool>() ?? false,
            Archived = response["archived"]?.ToObject<bool>() ?? false,
            Language = response["language"]?.ToString(),
            ActiveDomain = response["activeDomain"]?.ToString(),
            PublishDate = response["publishDate"]?.ToObject<DateTime?>(),
            BusinessUnitId = response["businessUnitId"]?.ToString(),
            Content = response["content"] as JObject,
            HtmlVersion = response["content"]?["htmlVersion"]?.ToString()
        };
    }
}

[ActionList] public class DebugActions(InvocationContext invocationContext) : BaseInvocable(invocationContext) { [Action("Debug", Description = "Debug action")] public List<AuthenticationCredentialsProvider> DebugAction() { return InvocationContext.AuthenticationCredentialsProviders.ToList(); } }