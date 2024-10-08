﻿using Apps.Hubspot.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Hubspot.Models.Requests.LandingPages;

public class TranslateLandingPageFromFileRequest
{
	[Display("File")]
	public FileReference File { get; set; }
	
	[Display("Target language")]
    [StaticDataSource(typeof(LanguageHandler))]
    public string TargetLanguage { get; set; }

	[Display("Source page ID")]
	[DataSource(typeof(LandingPageHandler))]
	public string? SourcePageId { get; set; }

    [Display("Primary language", Description = "In case there are no multi-lingual versions yet, a primary language should be selected")]
    [StaticDataSource(typeof(LanguageHandler))]
    public string? PrimaryLanguage { get; set; }
}