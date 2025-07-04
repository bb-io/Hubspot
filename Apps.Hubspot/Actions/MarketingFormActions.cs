﻿using System.Net.Mime;
using Apps.Hubspot.Actions.Base;
using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Requests.Forms;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Files;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;
using Apps.Hubspot.Models.Requests.Files;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Hubspot.Actions;

[ActionList]
public class MarketingFormActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BaseActions(invocationContext, fileManagementClient)
{
    [Action("Search marketing forms", Description = "Search for marketing forms based on provided filters")]
    public async Task<ListResponse<MarketingFormDto>> SearchMarketingForms([ActionParameter] SearchFormsRequest input)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}" + input.BuildQuery();
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var result = await Client.Paginate<MarketingFormDto>(request);
        
        if (!string.IsNullOrEmpty(input.Language))
        {
            result = result.Where(x => x.Configuration.Language == input.Language).ToList();
        }
        
        return new(result);
    }

    [Action("Get marketing form", Description = "Get a marketing form by its ID")]
    public async Task<MarketingFormDto> GetMarketingForm([ActionParameter] MarketingFormRequest formRequest)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{formRequest}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
    }

    [Action("Create marketing form", Description = "Create a new marketing form")]
    public async Task<MarketingFormDto> CreateMarketingForm([ActionParameter] CreateMarketingFormRequest formRequest)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}";
        var request = new HubspotRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(formRequest.GetRequestBody());

        return await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
    }

    [Action("Get marketing form content as HTML",
        Description = "Get content of a specific marketing form in HTML format")]
    public async Task<FileResponse> GetMarketingFormAsHtml([ActionParameter] MarketingFormRequest formRequest,
        [ActionParameter][Display("Exclude title from file")] bool? ExcludeTitle)
    {
        var form = await GetMarketingForm(formRequest);
        var name = ExcludeTitle.HasValue && ExcludeTitle.Value ? "" : form.Name;
        var html = HtmlConverter.ToHtml(form.FieldGroups, name, form.Configuration.Language, form.Id, ContentTypes.Form);

        var file = await FileManagementClient.UploadAsync(new MemoryStream(html), MediaTypeNames.Text.Html,
            $"{formRequest.FormId}.html");

        return new()
        {
            File = file
        };
    }

    [Action("Update marketing form from HTML",
        Description = "Update a marketing form from HTML content")]
    public async Task<MarketingFormDto> UpdateMarketingFormFromHtml(
        [ActionParameter] UpdateMarketingFormRequest formRequest)
    {
        var file = await FileManagementClient.DownloadAsync(formRequest.File);
        var bytes = await file.GetByteData();
        string formId;
        if (formRequest.FormId is null)
        {
            var extractedFormId = HtmlConverter.ExtractBlackbirdId(bytes) ?? throw new PluginMisconfigurationException(
            "Could not extract form ID from HTML content. Please ensure that the form ID is present in the HTML content as meta tag with name 'blackbird-reference-id' or provide it as input to the action.");
            formId = extractedFormId;
        }
        else { formId = formRequest.FormId; }
        var form = await GetMarketingForm(new() { FormId = formId });
        
        var htmlEntity = HtmlConverter.ExtractFormHtmlEntities(bytes);
        var formName = String.IsNullOrEmpty(htmlEntity.FormName) ? form.Name : htmlEntity.FormName ;
        var fieldGroups = htmlEntity.FieldGroups.Select(x =>
        {
            var field = form.FieldGroups.SelectMany(y => y.Fields).FirstOrDefault(y => y.Name == x.Name);
            if (field != null)
            {
                if (x.Properties.TryGetValue("label", out var property))
                {
                    field.Label = property;
                }
                
                if (x.Properties.TryGetValue("placeholder", out property))
                {
                    field.Placeholder = property;
                }
                
                if (x.Properties.TryGetValue("description", out property))
                {
                    field.Description = property;
                }

                if (x.Properties.TryGetValue("fieldType", out property))
                {
                    field.FieldType = property;
                }
                else if (string.IsNullOrEmpty(field.FieldType))
                {
                    field.FieldType = x.Options != null && x.Options.Any() ? "dropdown" : "single_line_text";
                }

                if (x.Properties.TryGetValue("objectTypeId", out property))
                {
                    field.ObjectTypeId = property;
                }
                else if (string.IsNullOrEmpty(field.ObjectTypeId))
                {
                    field.ObjectTypeId = "0-1";
                }

                if (x.Options != null && field.Options != null)
                {
                    var options = x.Options.Select(z =>
                    {
                        var option = field.Options.FirstOrDefault(y => y.Value == z.Key)!;
                        if (option != null)
                        {
                            option.Label = z.Value;
                        }
                        return option;
                    }).ToList();
                    
                    field.Options = options;
                }
            }
            else
            {
                return new FieldGroupDto
                {
                    GroupType = "default_group",
                    RichTextType = "text",
                    Fields = new List<FieldDto>
                    {
                        new()
                        {
                            ObjectTypeId = x.Properties.GetValueOrDefault("objectTypeId") != null
                            ? MapToValidObjectTypeId(x.Properties.GetValueOrDefault("objectTypeId"))
                            : "0-1",
                            Name = x.Name,
                            Label = x.Properties.GetValueOrDefault("label") ?? string.Empty,
                            Placeholder = x.Properties.GetValueOrDefault("placeholder") ?? string.Empty,
                            Description = x.Properties.GetValueOrDefault("description") ?? string.Empty,
                            FieldType = x.Properties.GetValueOrDefault("fieldType") ?? "single_line_text",
                            Options = x.Options?.Select(z => new OptionDto
                            {
                                Value = z.Key,
                                Label = z.Value
                            }).ToList()
                        }
                    }
                };
            }
            
            var group = form.FieldGroups.FirstOrDefault(y => y.Fields.Any(z => z.Name == x.Name))!;
            return group;
        }).ToList();
        
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{formId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = formName,
                fieldGroups
            });
        
        return await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
    }

    private string MapToValidObjectTypeId(string inputTypeId)
    {
        var validObjectTypeIds = new HashSet<string> { "0-1", "0-2" };
        inputTypeId = inputTypeId?.ToLowerInvariant() ?? "0-1";
        return validObjectTypeIds.Contains(inputTypeId) ? inputTypeId : "0-1";
    }

    [Action("Create marketing form from HTML", Description = "Create a marketing form from a HTML file content")]
    public async Task<MarketingFormDto> CreateMarketingFormFromHtml([ActionParameter] FileRequest fileRequest,
        [ActionParameter] CreateMarketingFormFromHtmlRequest input)
    {
        var htmlFile = await FileManagementClient.DownloadAsync(fileRequest.File);
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(htmlFile);

        var extractedValues = Apps.Hubspot.Utils.Extensions.HtmlExtensions.ExtractHtmlValuesForForm(htmlDoc);
        var createRequestBody = new CreateMarketingFormFromHtmlRequest
        {
            Name = input.Name ?? extractedValues.Name,
            FormType = input.FormType ?? extractedValues.FormType,
            Archived = input.Archived ?? extractedValues.Archived,
            Language = input.Language ?? extractedValues.Language
        }.GetRequestBody();

        var request = new HubspotRequest(ApiEndpoints.MarketingFormsEndpoint, Method.Post, Creds)
         .WithJsonBody(createRequestBody);
        var response = await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);

        var updatedForm = await UpdateMarketingFormFromHtml(new UpdateMarketingFormRequest 
        {
            FormId = response.Id,
            File = fileRequest.File
        });

        return updatedForm;
    }
}