﻿// See https://aka.ms/new-console-template for more information
using Apps.Hubspot.Actions;
using Apps.Hubspot.Helpers;
using Blackbird.Applications.Sdk.Common.Authentication;
using Newtonsoft.Json;
using System.Text;

PageActions actions = new PageActions();
var pageId = "115935034531";
var credentials = new List<AuthenticationCredentialsProvider>()
    {
        new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            "Authorization",
            "Bearer CPKikPWGMRIQAgEAQAAAAQIAAAA4AAAAYBi63Z8KIIW7yBgowPlnMhRXlc7i1Mb3m64qdphuVJu2k7vlujo9ACAAQf8HAAAAAIAAAGB4wCCAAAAAAAAABAAAOAAAAM7D_wEBAAAAgAb8BwAAAPADAACAPwAAAAAAAAIACEIUNiwNZk3Wqkg3qMhlwon5yC39MitKA25hMVIAWgA")
    };

//var response = await actions.GetAllLandingPages(credentials);
//Console.WriteLine("response");

//var sitePageResponse = await actions.GetSitePageAsHtml(credentials, pageId);
var sitePageResponse = await actions.GetAllSitePagesNotInLanguage(credentials, "en", "nl-NL");
//var response = await actions.TranslateSitePageFromFile(credentials, new Apps.Hubspot.Models.Requests.TranslateFromFileRequest()
//{
//    File = sitePageResponse.File,
//    TargetLanguage = "nl-NL"
//});

Console.WriteLine(sitePageResponse);

//var result = await actions.GetSitePage(
//      credentials,
//    pageId);
//var resultantHtml = PageHelpers.ObjectToHtml(JsonConvert.DeserializeObject(result.LayoutSections.ToString()));
//using (StreamWriter w = new StreamWriter("EmptyHtmlFile.html"))
//{
//    w.WriteLine($"<!DOCTYPE html>\n<html>\n<head>\n<meta charset=\"utf-8\" />\n<title>{result.HtmlTitle}</title>\n</head>\n<body><div data-pageid=\"{pageId}\"></div>\n");
//    w.WriteLine(resultantHtml);
//    w.WriteLine("</body>\n</html>");
//}

//using (StreamReader r = new StreamReader("EmptyHtmlFile.html"))
//{
//    string fileHtml = r.ReadToEnd();
//    var response = await actions.TranslateSitePageFromFile(credentials, new Apps.Hubspot.Models.Requests.TranslateFromFileRequest() { File = Encoding.ASCII.GetBytes(fileHtml), TargetLanguage="sv-SE" });
//    //using (StreamReader jr = new StreamReader("htmlObj.json"))
//    //{
//    //    string json = jr.ReadToEnd();
//    //    var items = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
//    // var fileInfo = PageHelpers.ExtractParentInfo(Encoding.ASCII.GetBytes(fileHtml));
//    //var layoutSection = PageHelpers.HtmlToObject(fileInfo.Html);
//    //    items["layoutSection"] = layoutSection;
//    //    using (StreamWriter w = new StreamWriter("translated.Json"))
//    //    {
//    //        w.WriteLine(JsonConvert.SerializeObject(items));
//    //    }
//    //}
//}