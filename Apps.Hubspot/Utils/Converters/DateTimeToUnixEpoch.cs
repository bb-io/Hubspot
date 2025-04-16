using Newtonsoft.Json;

namespace Apps.Hubspot.Utils.Converters;

public class DateTimeToUnixEpoch : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var dateTime = (DateTime)value;
        writer.WriteValue(TimeSpan.FromTicks(new DateTime(2025, 01, 01).Subtract(DateTime.UnixEpoch).Ticks).TotalMilliseconds.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // This converter is only for serialization
    }
}