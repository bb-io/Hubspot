using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Dtos.Forms
{
    public class HtmlFormValues
    {
        public string? Name { get; set; }
        public string? FormType { get; set; }
        public string? Language { get; set; }
        public bool? Archived { get; set; }
    }
}
