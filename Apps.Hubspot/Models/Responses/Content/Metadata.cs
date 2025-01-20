using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Responses.Content;
public class Metadata
{
    [Display("Content type")]
    public string Type { get; set; }

    [Display("ID")]
    public string Id { get; set; }

    [Display("Language")]
    public string Language { get; set; }
}
