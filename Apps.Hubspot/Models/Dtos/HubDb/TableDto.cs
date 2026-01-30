using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Hubspot.Models.Dtos.HubDb
{
    public class TableDto
    {
        [JsonProperty("id")]
        [Display("Table ID")]
        public string Id { get; set; }

        [JsonProperty("name")]
        [Display("Table name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("published")]
        [Display("Is published")]
        public bool Published { get; set; }

        [JsonProperty("rowCount")]
        [Display("Row count")]
        public int RowCount { get; set; }

        [JsonProperty("columnCount")]
        [Display("Column count")]
        public int ColumnCount { get; set; }

        [JsonProperty("updatedAt")]
        [Display("Updated at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("createdAt")]
        [Display("Created at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("deleted")]
        [Display("Is deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("updatedBy")]
        [Display("Updated by")]
        public HubSpotUserDto UpdatedBy { get; set; }

        [JsonProperty("createdBy")]
        [Display("Created by")]
        public HubSpotUserDto CreatedBy { get; set; }

    }
    public class HubSpotUserDto
    {
        [JsonProperty("id")]
        [Display("User ID")]
        public string Id { get; set; }

        [JsonProperty("email")]
        [Display("Email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        [Display("First name")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        [Display("Last name")]
        public string LastName { get; set; }
    }
}
