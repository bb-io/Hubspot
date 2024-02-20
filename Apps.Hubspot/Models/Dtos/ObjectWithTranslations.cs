using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Dtos
{
    public class ObjectWithTranslations
    {
        public string Language { get; set; }
        public Dictionary<string, ObjectWithId> Translations { get; set; }
    }
}
