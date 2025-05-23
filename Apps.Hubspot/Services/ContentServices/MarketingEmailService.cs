﻿using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos;
using Apps.Hubspot.Models.Dtos.Emails;
using Apps.Hubspot.Models.Dtos.Pages;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Requests.Emails;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class MarketingEmailService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}".WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var response = await Client.Paginate<MarketingEmailDto>(request);

        return response.Select(x => new Metadata
        {
            Id = x.Id,
            Title = x.Name,
            Domain = x.ActiveDomain,
            Language = x.Language,
            State = x.State,
            Published = x.IsPublished,
            Type = ContentTypes.Email,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt ?? DateTime.MinValue
        }).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var email = await Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);

        return new()
        {
            Id = email.Id,
            Title = email.Name,
            Domain = email.ActiveDomain,
            Language = email.Language!,
            State = email.State,
            Published = email.IsPublished,
            Type = ContentTypes.Email,
            CreatedAt = email.CreatedAt,
            UpdatedAt = email.UpdatedAt ?? DateTime.MinValue
        };
    }

    public override Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        throw new PluginMisconfigurationException("This operation is not supported for marketing email content type. The Hubspot API does not provide translations for email content type.");
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var email = await Client.ExecuteWithErrorHandling<EmailContentDto>(request);
        var html = HtmlConverter.ToHtml(email.Content, email.Name, email.Language, id, ContentTypes.Email, null, null, null, email.BusinessUnitId);

        return new MemoryStream(html);
    }

    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var fileBytes = await stream.GetByteData();
        var blackbirdId = HtmlConverter.ExtractBlackbirdId(fileBytes);
        
        if (blackbirdId == null)
        {
            throw new PluginMisconfigurationException("Marketing email ID is required. Please make sure that HTML file was generated by Hubspot app or set 'Create new' to true to create a new email instead.");
        }

        var titleText = HtmlConverter.ExtractTitle(fileBytes);
        var language = HtmlConverter.ExtractLanguage(fileBytes);
        var businessUnitId = HtmlConverter.ExtractBusinessUnitId(fileBytes);

        using var stringStream = new MemoryStream(fileBytes);
        var (pageInfo, json) = HtmlConverter.ToJson(stringStream);

        var updatedContent = new Content
        {
            FlexAreas = json["flexAreas"] as JObject,
            Widgets = json["widgets"] as JObject,
            StyleSettings = json["styleSettings"] as JObject,
            TemplatePath = json["templatePath"]?.ToString(),
            PlainTextVersion = json["plainTextVersion"]?.ToString() ?? ""
        };
        
        if (uploadContentRequest.CreateNew == true)
        {
            return await CreateNewEmailFromHtmlAsync(blackbirdId, titleText, targetLanguage ?? language, businessUnitId, updatedContent);
        }
        else
        {
            return await UpdateExistingEmailFromHtmlAsync(blackbirdId, titleText, language, businessUnitId, updatedContent);
        }
    }

    private async Task<Metadata> CreateNewEmailFromHtmlAsync(string emailId, string? titleText, string language, string? businessUnitId, Content updatedContent)
    {
        var email = await GetEmail(emailId);

        var createMarketingEmailRequest = new CreateMarketingEmailRequest
        {
            Name = string.IsNullOrEmpty(titleText) ? emailId : titleText,
            Language = language,
            BusinessUnitId = businessUnitId,
            Subject = email.Subject
        };

        var createRequest = new HubspotRequest(ApiEndpoints.MarketingEmailsEndpoint, Method.Post, Creds)
            .WithJsonBody(createMarketingEmailRequest, JsonConfig.Settings);

        if (!string.IsNullOrEmpty(createMarketingEmailRequest.BusinessUnitId))
        {
            createRequest.AddQueryParameter("businessUnitId", createMarketingEmailRequest.BusinessUnitId);
        }

        var newEmail = await Client.ExecuteWithErrorHandling<MarketingEmailDto>(createRequest);

        var updateEmailDto = new MarketingEmailOptionalRequest
        {
            Content = updatedContent
        };

        var updateEmailEndpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{newEmail.Id}";
        var updateEmailRequest = new HubspotRequest(updateEmailEndpoint, Method.Patch, Creds)
            .WithJsonBody(updateEmailDto, JsonConfig.Settings);

        var emailDto = await Client.ExecuteWithErrorHandling<MarketingEmailDto>(updateEmailRequest);

        return new()
        {
            Id = emailDto.Id,
            Title = emailDto.Name,
            Domain = emailDto.ActiveDomain,
            Language = emailDto.Language,
            State = emailDto.State,
            Published = emailDto.IsPublished,
            Type = ContentTypes.Email,
            CreatedAt = emailDto.CreatedAt,
            UpdatedAt = emailDto.UpdatedAt ?? DateTime.MinValue
        };
    }

    private async Task<Metadata> UpdateExistingEmailFromHtmlAsync(string emailId, string? titleText, string? language, string? businessUnitId, Content updatedContent)
    {
        var email = await GetEmail(emailId);

        var requestPayload = new MarketingEmailOptionalRequest
        {
            Name = string.IsNullOrEmpty(titleText) ? email.Name : titleText,
            Language = string.IsNullOrEmpty(language) ? email.Language : language,
            BusinessUnitId = string.IsNullOrEmpty(businessUnitId) ? email.BusinessUnitId : businessUnitId,
            Content = updatedContent
        };

        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{emailId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(requestPayload, JsonConfig.Settings);

        var emailDto = await Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);

        return new()
        {
            Id = emailDto.Id,
            Title = emailDto.Name,
            Domain = emailDto.ActiveDomain,
            Language = emailDto.Language,
            State = emailDto.State,
            Published = emailDto.IsPublished,
            Type = ContentTypes.Email,
            CreatedAt = emailDto.CreatedAt,
            UpdatedAt = emailDto.UpdatedAt ?? DateTime.MinValue
        };
    }

    public override async Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{id}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = updateContentRequest.Title
            }, JsonConfig.Settings);

        var email = await Client.ExecuteWithErrorHandling<MarketingEmailDto>(request);

        return new()
        {
            Id = email.Id,
            Title = email.Name,
            Domain = email.ActiveDomain,
            Language = email.Language,
            State = email.State,
            Published = email.IsPublished,
            Type = ContentTypes.Email,
            CreatedAt = email.CreatedAt,
            UpdatedAt = email.UpdatedAt ?? DateTime.MinValue
        };
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    private Task<EmailContentDto> GetEmail(string emailId)
    {
        var endpoint = $"{ApiEndpoints.MarketingEmailsEndpoint}{emailId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<EmailContentDto>(request);
    }
}