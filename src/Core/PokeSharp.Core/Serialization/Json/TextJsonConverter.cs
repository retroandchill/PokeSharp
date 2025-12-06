using System.Text.Json;
using System.Text.Json.Serialization;
using PokeSharp.Core.Strings;

namespace PokeSharp.Core.Serialization.Json;

/// <summary>
/// A JSON converter for the <see cref="Text"/> struct, facilitating
/// serialization and deserialization with System.Text.Json.
/// </summary>
/// <remarks>
/// This converter allows the <see cref="Text"/> struct to be seamlessly serialized into and deserialized
/// from its string representation in JSON. It handles both conversions and ensures that invalid
/// or null values throw appropriate exceptions during deserialization.
/// </remarks>
/// <threadsafety>
/// Instances of this converter are typically thread-safe as it does not maintain internal state.
/// </threadsafety>
/// <seealso cref="Text"/>
/// <seealso cref="System.Text.Json.JsonSerializer"/>
public class TextJsonConverter : JsonConverter<Text>
{
    /// <inheritdoc />
    public override Text Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var foundString = reader.GetString();
            return foundString is not null ? Text.FromLocText(foundString) : throw new JsonException("Name cannot be null.");
        }
        catch (InvalidOperationException ex)
        {
            throw new JsonException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Text value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToLocString());
    }
}
