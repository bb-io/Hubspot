using Apps.Hubspot.DataSourceHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Requests
{
    public class LanguageVariationRequest
    {
        public string Id { get; set; }
        public string Language { get; set; }
        public string? PrimaryLanguage { get; set; }
    }
}
