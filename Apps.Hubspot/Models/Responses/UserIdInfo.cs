using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Responses
{
   public class UserIdInfo
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
