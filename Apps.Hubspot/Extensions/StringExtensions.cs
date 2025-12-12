using System.Text;

namespace Apps.Hubspot.Extensions;
public static class StringExtensions
{
    public static string SanitizeFileName(
            this string input,
            string replacement = "")
    {
        if (string.IsNullOrWhiteSpace(input))
            return "file";

        var invalidChars = Path.GetInvalidFileNameChars();

        var sanitized = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            sanitized.Append(invalidChars.Contains(ch) ? replacement : ch);
        }
        var result = sanitized.ToString();

        return string.IsNullOrEmpty(result) ? "file" : result;
    }
}
