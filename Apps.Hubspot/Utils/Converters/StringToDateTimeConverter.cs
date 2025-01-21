namespace Apps.Hubspot.Utils.Converters;

public static class StringToDateTimeConverter
{
    public static DateTime ToDateTime(string? date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return DateTime.MinValue;
        }

        if (DateTime.TryParse(date, out var dateTime))
        {
            return dateTime;
        }

        return DateTime.MinValue;
    }
}