
using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests;

    public class LocalizablePropertiesRequest
    {
        [Display("Properties to include")]
        public IEnumerable<string>? PropertiesToInclude { get; set; }

        [Display("Properties to exclude")]
        public IEnumerable<string>? PropertiesToExclude { get; set; }
    }

