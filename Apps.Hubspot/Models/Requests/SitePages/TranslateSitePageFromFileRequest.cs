﻿using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.SitePages;

public class TranslateSitePageFromFileRequest
{
    public FileReference File { get; set; }
    
    [Display("Target language")]
    [StaticDataSource(typeof(LanguageHandler))]
    public string TargetLanguage { get; set; }

    [DataSource(typeof(SitePageHandler))]
    [Display("Source page ID")]
    public string? SourcePageId { get; set; }

    [Display("Primary language", Description = "In case there are no multi-lingual versions yet, a primary language should be selected")]
    [StaticDataSource(typeof(LanguageHandler))]
    public string? PrimaryLanguage { get; set; }
}