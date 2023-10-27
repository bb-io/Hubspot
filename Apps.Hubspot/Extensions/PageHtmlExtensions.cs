using Apps.Hubspot.Constants;

namespace Apps.Hubspot.Extensions;

public static class PageHtmlExtensions
{
    public static string AsPageHtml(this (string title, string? language, string body) tuple)
        => string.Format(Formats.PageHtmlFormat, tuple.title, tuple.language, tuple.body);
}