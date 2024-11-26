using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Dtos.Business_units
{
    public class BusinessUnitDto
    {
        [Display("Business unit ID")]
        [JsonProperty("id")]
        public string BusinessUnitId {  get; set; }
        [Display("Names of business units", Description= "The names of Business Units to retrieve. If empty or not provided, then all associated Business Units will be returned.")]
        [JsonProperty("name")]
        public string Name { get; set; }
        //[Display("Logo meta data")]
        //public LogoMetadata LogoMetadata { get; set; }

    }

    public class LogoMetadata
    {
        [Display("Alternative logo text")]
        public string LogoAltText { get; set; }
        [Display("Url of logo")]
        public string ResizeUrl {  get; set; }
        [Display("Main logo url")]
        public string LogoUrl { get; set; }
    }
}
