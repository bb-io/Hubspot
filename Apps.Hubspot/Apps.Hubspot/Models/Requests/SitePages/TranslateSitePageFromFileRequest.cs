﻿using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Hubspot.Models.Requests.SitePages;

public class TranslateSitePageFromFileRequest
{
    public File File { get; set; }
    
    [Display("Target language")]
    public string TargetLanguage { get; set; }

    [DataSource(typeof(SitePageHandler))]
    [Display("Source page")]
    public string SourcePageId { get; set; }
}