using Blackbird.Applications.Sdk.Common;

namespace Apps.Hubspot.Models.Requests;

public class UpdatedAfterRequest
{
    [Display("Updated after")]
    public DateTime UpdatedAfter { get; set; }
}