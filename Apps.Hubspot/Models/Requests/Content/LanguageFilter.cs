using Apps.Hubspot.DataSourceHandlers;
using Apps.Hubspot.DataSourceHandlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Requests.Content;
public class LanguageFilter
{
    [Display("Language")]
    [StaticDataSource(typeof(LanguageHandler))]
    public string ContentTypes { get; set; }
}
