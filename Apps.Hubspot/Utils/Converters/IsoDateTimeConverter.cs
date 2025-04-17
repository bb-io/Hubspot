using Newtonsoft.Json;

namespace Apps.Hubspot.Utils.Converters;

public class IsoDateTimeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(DateTime?) || objectType == typeof(DateTime));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var dateTime = (DateTime)value;
        writer.WriteValue(dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFF'Z'"));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // This converter is only for serialization
    }
}