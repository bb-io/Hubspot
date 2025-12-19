using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Hubspot.Models.Requests.HubDb
{
    public class SearchTablesRequest
    {
        [Display("Version")]
        [StaticDataSource(typeof(TableVersionBothHandler))]
        public string Version { get; set; }

        [Display("Name contains")]
        public string? NameContains { get; set; }

        [Display("Label contains")]
        public string? LabelContains { get; set; }

        [Display("Updated after")]
        public DateTime? UpdatedFrom { get; set; }

        [Display("Updated before")]
        public DateTime? UpdatedTo { get; set; }

        [Display("Created after")]
        public DateTime? CreatedFrom { get; set; }

        [Display("Created before")]
        public DateTime? CreatedTo { get; set; }

    }
}
