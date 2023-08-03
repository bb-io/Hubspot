using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Apps.Hubspot
{
    public class StringConverter : System.Text.Json.Serialization.JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            if (reader.TokenType == JsonTokenType.Number)
            {
                var stringValue = reader.GetInt32();
                return stringValue.ToString();
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            throw new System.Text.Json.JsonException();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

    }
}
