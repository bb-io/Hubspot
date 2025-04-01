﻿using Apps.Hubspot.Api;
using Apps.Hubspot.Constants;
using Apps.Hubspot.HtmlConversion;
using Apps.Hubspot.Models.Dtos.Forms;
using Apps.Hubspot.Models.Requests.Content;
using Apps.Hubspot.Models.Requests.Forms;
using Apps.Hubspot.Models.Responses;
using Apps.Hubspot.Models.Responses.Content;
using Apps.Hubspot.Services.ContentServices.Abstract;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Hubspot.Services.ContentServices;

public class MarketingFormService(InvocationContext invocationContext) : BaseContentService(invocationContext)
{
    public override async Task<List<Metadata>> SearchContentAsync(Dictionary<string, string> query)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}".WithQuery(query);
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        var response = await Client.Paginate<MarketingFormDto>(request);

        return response.Select(x => new Metadata
        {
            Id = x.Id,
            Title = x.Name,
            Domain = "NONE",
            Language = x.Configuration?.Language ?? string.Empty,
            State = "PUBLISHED",
            Published = true,
            Type = ContentTypes.Form,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();
    }

    public override async Task<Metadata> GetContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var form = await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);

        return new()
        {
            Id = form.Id,
            Title = form.Name,
            Domain = "NONE",
            Language = form.Configuration?.Language ?? string.Empty,
            State = "PUBLISHED",
            Published = true,
            Type = ContentTypes.Form,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt
        };
    }

    public override Task<TranslatedLocalesResponse> GetTranslationLanguageCodesAsync(string id)
    {
        throw new PluginMisconfigurationException("This operation is not supported for marketing form content type. The Hubspot API does not provide translations for from content type.");
    }

    public override async Task<Stream> DownloadContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{id}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);
        var form = await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);

        var htmlBytes = HtmlConverter.ToHtml(form.FieldGroups, form.Name, form.Configuration.Language, form.Id, ContentTypes.Form);
        return new MemoryStream(htmlBytes);
    }

    public override async Task<Metadata> UpdateContentFromHtmlAsync(string targetLanguage, Stream stream, UploadContentRequest uploadContentRequest)
    {
        var bytes = await stream.GetByteData();

        var extractedFormId = HtmlConverter.ExtractBlackbirdId(bytes);
        if (extractedFormId == null)
        {
            throw new PluginMisconfigurationException(
                "Could not extract form ID from HTML content. Please ensure that the form ID is present in the HTML content as meta tag with name 'blackbird-reference-id', or set 'Create new' to true to create a new form instead.");
        }

        var htmlEntity = HtmlConverter.ExtractFormHtmlEntities(bytes);

        if (uploadContentRequest.CreateNew == true)
        {
            return await CreateNewFormFromHtmlAsync(htmlEntity, extractedFormId!, targetLanguage);
        }
        else
        {
            return await UpdateExistingFormFromHtmlAsync(extractedFormId!, htmlEntity);
        }
    }

    private async Task<Metadata> CreateNewFormFromHtmlAsync(FormHtmlEntities htmlEntity, string originalFormId, string targetLanguage)
    {
        // Get the original form to use as a template
        var originalForm = await GetMarketingForm(new() { FormId = originalFormId });
        
        // Create a new form with the target language
        var createEndpoint = $"{ApiEndpoints.MarketingFormsEndpoint}";
        var createRequest = new HubspotRequest(createEndpoint, Method.Post, Creds)
            .WithJsonBody(new
            {
                name = htmlEntity.FormName ?? "New Marketing Form",
                formType = originalForm.FormType,
                createdAt = DateTime.UtcNow,
                configuration = new
                {
                    language = targetLanguage
                }
            });

        var newForm = await Client.ExecuteWithErrorHandling<MarketingFormDto>(createRequest);
        
        // Transform the field groups based on the HTML entity
        var fieldGroups = TransformFieldGroups(htmlEntity.FieldGroups, originalForm.FieldGroups);
        
        // Update the newly created form with the transformed field groups
        var updateEndpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{newForm.Id}";
        var updateRequest = new HubspotRequest(updateEndpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = htmlEntity.FormName,
                fieldGroups
            });

        var updatedForm = await Client.ExecuteWithErrorHandling<MarketingFormDto>(updateRequest);
        
        return ConvertToMetadata(updatedForm);
    }

    private async Task<Metadata> UpdateExistingFormFromHtmlAsync(string formId, FormHtmlEntities htmlEntity)
    {
        // Get the existing form
        var existingForm = await GetMarketingForm(new() { FormId = formId });
        
        // Transform the field groups based on the HTML entity
        var fieldGroups = TransformFieldGroups(htmlEntity.FieldGroups, existingForm.FieldGroups);
        
        // Update the existing form with the transformed field groups
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{formId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = htmlEntity.FormName,
                fieldGroups
            });

        var updatedForm = await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
        
        return ConvertToMetadata(updatedForm);
    }
    
    private List<FieldGroupDto> TransformFieldGroups(List<FormHtmlEntity> htmlFieldEntities, List<FieldGroupDto> existingFieldGroups)
    {
        var existingFields = existingFieldGroups.SelectMany(g => g.Fields).ToList();
        
        return htmlFieldEntities.Select(htmlField =>
        {
            var existingField = existingFields.FirstOrDefault(f => f.Name == htmlField.Name);
            
            if (existingField != null)
            {
                // Update existing field properties if they exist in the HTML entity
                if (htmlField.Properties.TryGetValue("label", out var label))
                    existingField.Label = label;

                if (htmlField.Properties.TryGetValue("placeholder", out var placeholder))
                    existingField.Placeholder = placeholder;
                    
                if (htmlField.Properties.TryGetValue("description", out var description))
                    existingField.Description = description;
                
                // Update field options if they exist
                if (htmlField.Options != null && existingField.Options != null)
                {
                    existingField.Options = htmlField.Options
                        .Select(htmlOption => {
                            var matchingOption = existingField.Options.FirstOrDefault(o => o.Value == htmlOption.Key);
                            if (matchingOption != null)
                            {
                                matchingOption.Label = htmlOption.Value;
                                return matchingOption;
                            }
                            return null;
                        })
                        .Where(o => o != null)
                        .Select(o => o!) 
                        .ToList();
                }
                
                // Return the existing field group that contains this field
                return existingFieldGroups.FirstOrDefault(g => g.Fields.Any(f => f.Name == htmlField.Name)) 
                    ?? CreateNewFieldGroup(htmlField);
            }
            else
            {
                // Create a new field group if the field doesn't exist
                return CreateNewFieldGroup(htmlField);
            }
        }).ToList();
    }
    
    private FieldGroupDto CreateNewFieldGroup(FormHtmlEntity htmlField)
    {
        return new FieldGroupDto
        {
            GroupType = "default_group",
            RichTextType = "text",
            Fields = new List<FieldDto>
            {
                new()
                {
                    Name = htmlField.Name,
                    Label = htmlField.Properties.GetValueOrDefault("label") ?? string.Empty,
                    Placeholder = htmlField.Properties.GetValueOrDefault("placeholder") ?? string.Empty,
                    Description = htmlField.Properties.GetValueOrDefault("description") ?? string.Empty,
                    Options = htmlField.Options?.Select(option => new OptionDto
                    {
                        Value = option.Key,
                        Label = option.Value
                    }).ToList()
                }
            }
        };
    }
    
    private Metadata ConvertToMetadata(MarketingFormDto form)
    {
        return new Metadata
        {
            Id = form.Id,
            Title = form.Name,
            Domain = "NONE",
            Language = form.Configuration?.Language ?? string.Empty,
            State = "PUBLISHED",
            Published = true,
            Type = ContentTypes.Form,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt
        };
    }

    public override async Task<Metadata> UpdateContentAsync(string id, UpdateContentRequest updateContentRequest)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{id}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = updateContentRequest.Title,
            });

        var form = await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
        return new()
        {
            Id = form.Id,
            Title = form.Name,
            Domain = "NONE",
            Language = form.Configuration?.Language ?? string.Empty,
            State = "PUBLISHED",
            Published = true,
            Type = ContentTypes.Form,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt
        };
    }

    public override Task DeleteContentAsync(string id)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{id}";
        var request = new HubspotRequest(endpoint, Method.Delete, Creds);
        return Client.ExecuteWithErrorHandling(request);
    }

    private async Task<MarketingFormDto> GetMarketingForm(MarketingFormRequest formRequest)
    {
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{formRequest.FormId}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
    }
}