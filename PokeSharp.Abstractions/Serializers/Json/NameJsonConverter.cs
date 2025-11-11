using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeSharp.Abstractions.Serializers.Json;

public class NameJsonConverter : JsonConverter<Name>
{
    public override Name Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        try
        {
            var foundString = reader.GetString();
            return foundString is not null
                ? new Name(foundString)
                : throw new JsonException("Name cannot be null.");
        }
        catch (InvalidOperationException ex)
        {
            throw new JsonException(ex.Message, ex);
        }
    }

    public override void Write(Utf8JsonWriter writer, Name value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
