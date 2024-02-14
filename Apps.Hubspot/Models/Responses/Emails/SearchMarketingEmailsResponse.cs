using Apps.Hubspot.Models.Dtos.Emails;

namespace Apps.Hubspot.Models.Responses.Emails;

public record SearchMarketingEmailsResponse(List<MarketingEmailDto> Emails);