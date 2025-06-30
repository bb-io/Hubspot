using Newtonsoft.Json;
using System.Globalization;

namespace Apps.Hubspot.Utils.Converters;

public class DateTimeToTimestamp : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(DateTime?) || objectType == typeof(DateTime));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var dateTime = (DateTime)value;
        var timestamp = TimeSpan.FromTicks(dateTime.Subtract(DateTime.UnixEpoch).Ticks).TotalMilliseconds;
        writer.WriteValue(timestamp.ToString("F0", CultureInfo.InvariantCulture));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // This converter is only for serialization
    }
}