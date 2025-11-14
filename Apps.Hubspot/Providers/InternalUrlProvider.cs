using Apps.Hubspot.Models.Entities;
using Blackbird.Applications.Sdk.Common.Exceptions;
using RestSharp;
using System.Text.RegularExpressions;

namespace Apps.Hubspot.Providers;

public static class InternalUrlProvider
{
    public static HtmlVariablesEntity? GetHtmlVariables(string url)
    {
        try
        {
            // Validate URL format before making request
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return null;
            }

            var restClient = new RestClient(url);
            var request = new RestRequest(string.Empty, Method.Get);

            var response = restClient.Execute(request);
            if (response.IsSuccessful)
            {
                var htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(response.Content);

                var scriptNodes = htmlDocument.DocumentNode.SelectNodes("//script");
                if (scriptNodes != null)
                {
                    foreach (var scriptNode in scriptNodes)
                    {
                        if (scriptNode.InnerText.Contains("var hsVars =") && scriptNode.InnerText.Contains("render_id") && scriptNode.InnerText.Contains("page_id"))
                        {
                            var scriptContent = scriptNode.InnerText;
                            
                            string pageId = ExtractValue(scriptContent, "page_id");
                            if (string.IsNullOrEmpty(pageId))
                            {
                                pageId = ExtractValue(scriptContent, "analytics_page_id");
                            }
                            
                            string pageType = ExtractValue(scriptContent, "analytics_page_type");
                            return new HtmlVariablesEntity
                            {
                                PageId = pageId,
                                PageType = pageType
                            };
                        }
                    }
                }
                
                return null;
            }
            else
            {
                // Return null instead of throwing exception for non-critical operation
                return null;
            }
        }
        catch (UriFormatException)
        {
            // Invalid URL format, return null
            return null;
        }
        catch (Exception)
        {
            // Any other error during URL fetching, return null
            return null;
        }
    }
    
    private static string ExtractValue(string scriptContent, string key)
    {
        var pattern = $@"{key}\s*:\s*([^,\r\n}}]+)";
        var match = Regex.Match(scriptContent, pattern);
        
        if (match.Success)
        {
            var value = match.Groups[1].Value.Trim();
            return value.Trim('"', '\'');
        }
        
        return string.Empty;
    }
}