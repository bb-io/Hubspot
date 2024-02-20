namespace Apps.Hubspot.Models.Responses;

public record ListResponse<T>(IEnumerable<T> Items);
