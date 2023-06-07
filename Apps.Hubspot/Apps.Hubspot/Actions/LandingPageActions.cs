#region General Imports
using RestSharp;
using System.Text;
using Newtonsoft.Json;
#endregion

#region Internal Imports
using Apps.Hubspot.Models;
using Apps.Hubspot.Helpers;
using Apps.Hubspot.Constants;
using Apps.Hubspot.Dtos.Pages;
#endregion

#region SDK Imports
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Hubspot.Models.Requests;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
#endregion

namespace Apps.Hubspot.Actions
{
    public partial class PageActions
    {

        [Action("Get all landing pages", Description = "Get a list of all landing pagess")]
        public async Task<GetAllResponse<PageDto>> GetAllLandingPages(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest(PageConstants.LandingPages(), Method.Get, authenticationCredentialsProviders);
            GetAllResponse<PageDto>? getAllResponse = await client.GetAsync<GetAllResponse<PageDto>>(request);
            return getAllResponse;
        }


        [Action("Get all landing pages with language X", Description = "Get a list of all pages in a specified language")]
        public async Task<GetAllResponse<PageDto>> GetAllLandingPagesInLanguage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string language)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"{PageConstants.LandingPages()}?language={language}", Method.Get, authenticationCredentialsProviders);
            GetAllResponse<PageDto>? getAllResponse = await client.GetAsync<GetAllResponse<PageDto>>(request);
            return getAllResponse;
        }


        [Action("Get all landing pages not translated in language X", Description = "Get a list of all pages not translated in specified language")]
        public async Task<List<PageDto>> GetAllLandingPagesNotInLanguage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string primaryLanguage, [ActionParameter] string language)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest($"{PageConstants.LandingPages()}?property=language,id,translations", Method.Get, authenticationCredentialsProviders);
            GetAllResponse<PageDto>? getAllResponse = await client.GetAsync<GetAllResponse<PageDto>>(request);
            // filter all the pages that are in primary language or they are never translated
            var result = getAllResponse.Results.Where(p => p.Language is null || p.Language == primaryLanguage).ToList();
            // filter the pages that do not have the specified language in the translations
            result = result.Where(p => p.Language is null || !((JObject)JsonConvert.DeserializeObject(p.Translations.ToString())).Properties().Any(t => t.Name == language.ToLower())).ToList();
            return result;
        }

        [Action("Get all landing pages udated after datetime", Description = "Get a list of all landing pagess that were updated after the given date time. Date time is exclusive")]
        public async Task<GetAllResponse<PageDto>> GetAllLandingPagesAfter(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, DateTime updatedAfter)
        {
            var client = new HubspotClient(authenticationCredentialsProviders);
            var request = new HubspotRequest(PageConstants.LandingPages(updatedAfter), Method.Get, authenticationCredentialsProviders);
            GetAllResponse<PageDto>? getAllResponse = await client.GetAsync<GetAllResponse<PageDto>>(request);
            return getAllResponse;
        }

        [Action("Get a landing page", Description = "Get information of a specific landing page")]
        public async Task<PageDto?> GetLandingPage(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string pageId)
        {
            var result = await GetLPage(authenticationCredentialsProviders, pageId);
            return result;
        }

        [Action("Get a site page as HTML file", Description = "Get information of a specific page and return an HTML file of its content")]
        public async Task<FileResponse> GetLandingPageAsHtml(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] string pageId)
        {
            var result = await GetLPage(authenticationCredentialsProviders, pageId);
            var htmlStringBuilder = PageHelpers.ObjectToHtml(JsonConvert.DeserializeObject(result.LayoutSections.ToString()));
            string htmlFile = $"<!DOCTYPE html>\n<html>\n<head>\n<meta charset=\"utf-8\" />\n<title>{result.HtmlTitle}</title>\n</head>\n<body><div lang=\"{result.Language}\"></div>\n{htmlStringBuilder.ToString()}</body></html>";

            return new FileResponse()
            {
                File = Encoding.ASCII.GetBytes(htmlFile),
                FileName = $"{pageId}.html",
                FileLanguage = result.Language,
                Id = pageId
            };
        }

        [Action("Translate a site page from HTML file", Description = "Create a new translation for a site page based on a file input")]
        public async Task<BaseResponse> TranslateLandingPageFromFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] TranslateFromFileRequest request)
        {
            try
            {
                var pageInfo = PageHelpers.ExtractParentInfo(request.File);
                // Create a Translation of the parent page
                var translationResponse = await CreateLandingPageTranslation(authenticationCredentialsProviders, request.SourcePageId, pageInfo.Language, request.TargetLanguage);

                // Update the layout section of translated page
                var updateDto = new PageBaseDto()
                {
                    Id = translationResponse.Id,
                    HtmlTitle = pageInfo.Title,
                    LayoutSections = PageHelpers.HtmlToObject(pageInfo.Html, JsonConvert.DeserializeObject(translationResponse.LayoutSections.ToString()))
                };
                // Update the translated page
                var updateResponse = await UpdateTranslatedLandingPage(authenticationCredentialsProviders, updateDto);

                return new BaseResponse()
                {
                    StatusCode = ((int)updateResponse.StatusCode),
                    Details = string.Empty,
                    Id = translationResponse.Id
                };
            }
            catch (Exception ex)
            {
                Logger.LogJson(ex);
                throw;
            }
        }

        [Action("Schedule a site-page for publishing", Description = "Schedules a site page for publishing on the given time")]
        public async Task<BaseResponse> ScheduleALandingPageForPublish(IEnumerable<AuthenticationCredentialsProvider> providers,
            [ActionParameter] PublishPageReqeuest request)
        {
            // Publish the translaged page
            var publishResponse = await PublishLandingPage(providers, request.Id, request.DateTime == null ? request.DateTime.Value : DateTime.Now.AddSeconds(30));
            return new BaseResponse()
            {
                StatusCode = ((int)publishResponse.StatusCode),
                Details = publishResponse.Content,
                Id = request.Id
            };
        }

        #region PRIVATE METHODS

        private async Task<PageDto?> GetLPage(IEnumerable<AuthenticationCredentialsProvider> providers, string pageId)
        {
            var client = new HubspotClient(providers);
            var request = new HubspotRequest(PageConstants.ALandingPage(pageId), Method.Get, providers);
            var page = await client.GetAsync<PageDto>(request);
            return page;
        }

        private async Task<PageDto?> CreateLandingPageTranslation(IEnumerable<AuthenticationCredentialsProvider> providers, string pageId, string primaryLanguage, string targetLanguage)
        {
            try
            {
                var client = new HubspotClient(providers);
                var request = new HubspotRequest(PageConstants.CreateLandingPageTranslation, Method.Post, providers);

                var translationRequestBody = new CreateTranslationRequest
                {
                    Id = pageId
                };
                translationRequestBody.SetLanguage(new System.Globalization.CultureInfo(primaryLanguage), new System.Globalization.CultureInfo(targetLanguage));
                request.AddJsonBody(translationRequestBody);

                var response = await client.PostAsync<PageDto>(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task<RestResponse> UpdateTranslatedLandingPage(IEnumerable<AuthenticationCredentialsProvider> providers, PageBaseDto page)
        {
            try
            {
                var client = new HubspotClient(providers);
                var request = new HubspotRequest(PageConstants.UpdateLandingPage(page.Id), Method.Patch, providers);

                var jsonString = JsonConvert.SerializeObject(page, serializationSettings);

                request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
                var response = await client.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task<RestResponse> PublishLandingPage(IEnumerable<AuthenticationCredentialsProvider> providers, string pageId, DateTime dateTime)
        {
            try
            {
                var client = new HubspotClient(providers);
                var request = new HubspotRequest(PageConstants.PublishLandingPage, Method.Post, providers);

                var jsonString = JsonConvert.SerializeObject(new {id = pageId, publishDate = dateTime}, serializationSettings);

                request.AddParameter("application/json", jsonString, ParameterType.RequestBody);
                var response = await client.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion
    }
}
