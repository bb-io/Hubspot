using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Hubspot.Models.Requests
{
    public class AccessTokenInfo
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("hub_id")]
        public int HubId { get; set; }

        [JsonProperty("app_id")]
        public int AppId { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }

        [JsonProperty("hub_domain")]
        public string HubDomain { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

}
