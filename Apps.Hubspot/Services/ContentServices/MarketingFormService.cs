using Apps.Hubspot.Api;
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
        
        var extractedFormId = HtmlConverter.ExtractBlackbirdId(bytes) ?? throw new PluginMisconfigurationException(
            "Could not extract form ID from HTML content. Please ensure that the form ID is present in the HTML content as meta tag with name 'blackbird-reference-id'.");

        var form = await GetMarketingForm(new() { FormId = extractedFormId });
        
        var htmlEntity = HtmlConverter.ExtractFormHtmlEntities(bytes);
        form.Name = htmlEntity.FormName;
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
                
                if(x.Options != null && field.Options != null)
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
                            Name = x.Name,
                            Label = x.Properties.GetValueOrDefault("label") ?? string.Empty,
                            Placeholder = x.Properties.GetValueOrDefault("placeholder") ?? string.Empty,
                            Description = x.Properties.GetValueOrDefault("description") ?? string.Empty,
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
        
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{extractedFormId}";
        var request = new HubspotRequest(endpoint, Method.Patch, Creds)
            .WithJsonBody(new
            {
                name = form.Name,
                fieldGroups
            });
        
        var marketingFormDto = await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
        return new()
        {
            Id = marketingFormDto.Id,
            Title = marketingFormDto.Name,
            Domain = "NONE",
            Language = marketingFormDto.Configuration?.Language ?? string.Empty,
            State = "PUBLISHED",
            Published = true,
            Type = ContentTypes.Form,
            CreatedAt = marketingFormDto.CreatedAt,
            UpdatedAt = marketingFormDto.UpdatedAt
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
        var endpoint = $"{ApiEndpoints.MarketingFormsEndpoint}/{formRequest}";
        var request = new HubspotRequest(endpoint, Method.Get, Creds);

        return await Client.ExecuteWithErrorHandling<MarketingFormDto>(request);
    }
}