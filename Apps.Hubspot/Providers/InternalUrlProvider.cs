using Apps.Hubspot.Models.Entities;
using Blackbird.Applications.Sdk.Common.Exceptions;
using RestSharp;
using System.Text.RegularExpressions;

namespace Apps.Hubspot.Providers;

public static class InternalUrlProvider
{
    public static HtmlVariablesEntity? GetHtmlVariables(string url)
    {
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
            throw new PluginApplicationException($"Failed to fetch HTML variables. Status code: {response.StatusCode}, Error: {response.ErrorMessage}. Please make sure the URL ({url}) is correct and accessible.");
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