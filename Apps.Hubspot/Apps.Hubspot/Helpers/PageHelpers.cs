using System.Text;
using Apps.Hubspot.Models.Responses.Pages;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Apps.Hubspot.Helpers;

public static class PageHelpers
{
    #region PUBLIC AREA

    /// <summary>
    /// Converts a JObject to HTML by extracting its params, name and rows data
    /// </summary>
    /// <param name="layoutObject">Object to convert</param>
    /// <returns>HTML in string builder instance</returns>
    public static string ObjectToHtml(JObject layoutObject)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in layoutObject.Properties())
        {
            var html = GetHtml((JObject)item.Value);
            sb.Append(html);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Updates the Object with the HTML from html string
    /// </summary>
    /// <param name="doc">string to replace content in layoutObject</param>
    /// <param name="layoutObject">Object to replace content inside the html props</param>
    /// <returns>Update jObject</returns>
    public static JObject HtmlToObject(HtmlDocument doc, JObject layoutObject)
    {
        foreach (var item in layoutObject.Properties())
            item.Value = FillTranslatedParamValueFromHtml((JObject)item.Value, doc);

        return layoutObject;
    }


    public static PageInfoResponse ExtractParentInfo(byte[] File)
    {
        var fileString = Encoding.UTF8.GetString(File);
        var doc = fileString.AsHtmlDocument();

        var pageId = doc.DocumentNode.SelectNodes("//div/@lang").FirstOrDefault();
        var title = doc.DocumentNode.SelectSingleNode("//title");

        return new()
        {
            Html = doc,
            Title = title?.InnerHtml,
            Language = pageId?.Attributes["lang"]?.Value
        };
    }

    #endregion

    #region PRIVATE AREA

    /// <summary>
    /// Recursively fills the params html value with updated/translated html
    /// </summary>
    /// <param name="rowObject"></param>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static JObject FillTranslatedParamValueFromHtml(JObject rowObject, HtmlDocument doc)
    {
        var propsToFilter = new List<string>() { "params", "rows", "name" };
        var strippedObject = rowObject.Properties().Where(p => propsToFilter.Contains(p.Name)).Select(p => p);
        foreach (var prop in strippedObject)
        {
            if (prop.Name == "params")
            {
                // search for html in params
                var htmlProp = ((JObject)prop.Value).Properties().Where(paramProp => paramProp.Name == "html")
                    .Select(paramProp => paramProp).FirstOrDefault();

                if (htmlProp is not null)
                {
                    var nameProperty = strippedObject.Where(p => p.Name == "name").Select(i => i).FirstOrDefault();
                    htmlProp.Value = GetHtmlValue(nameProperty.Value.ToString(), doc);
                }

                continue;
            }

            if (prop.Name == "rows")
            {
                foreach (var row in (prop.Value))
                foreach (var rowProp in ((JObject)row).Properties())
                    rowProp.Value = FillTranslatedParamValueFromHtml((JObject)rowProp.Value, doc);
            }
        }

        return rowObject;
    }

    /// <summary>
    /// Get's the div with given id from html document and returns the html as a string
    /// </summary>
    /// <param name="divId"></param>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static string GetHtmlValue(string divId, HtmlDocument doc)
    {
        var div = doc.DocumentNode.SelectSingleNode($"//div[@id=\'{divId}\']");
        return div?.InnerHtml ?? string.Empty;
    }

    /// <summary>
    /// Recursively converts the JObject to an HTML string
    /// </summary>
    /// <param name="rowObject"></param>
    /// <returns></returns>
    private static StringBuilder GetHtml(JObject rowObject)
    {
        var sb = new StringBuilder();
        var propsToFilter = new List<string>() { "params", "name", "rows" };

        var strippedObject = rowObject.Properties()
            .Where(p => propsToFilter.Contains(p.Name))
            .Select(p => p)
            .ToArray();

        foreach (var prop in strippedObject)
        {
            if (prop.Name == "params")
            {
                // search for html in params
                var htmlProp = ((JObject)prop.Value).Properties().Where(paramProp => paramProp.Name == "html")
                    .Select(paramProp => paramProp).FirstOrDefault();

                if (htmlProp != null)
                {
                    var nameProperty = strippedObject.Where(p => p.Name == "name").Select(i => i).FirstOrDefault();
                    sb.Append($"<div id=\"{nameProperty.Value.ToString()}\">{htmlProp.Value.ToString()}</div>");
                }
                
                continue;
            }
            
            foreach (var row in (prop.Value))
            {
                foreach (var rowProp in ((JObject)row).Properties())
                {
                    var builder = GetHtml((JObject)rowProp.Value);
                    sb.Append(builder);
                }
            }
        }

        return sb;
    }

    #endregion
}