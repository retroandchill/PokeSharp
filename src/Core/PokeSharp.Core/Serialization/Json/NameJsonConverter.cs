using System.Text.Json;
using System.Text.Json.Serialization;
using PokeSharp.Core.Strings;
using PokeSharp.Core.Utils;

namespace PokeSharp.Core.Serialization.Json;

/// <summary>
/// Custom JSON converter for the <see cref="Name"/> struct, enabling serialization
/// and deserialization to and from JSON string values.
/// </summary>
/// <remarks>
/// This class serializes a <see cref="Name"/> instance as a string representation
/// and deserializes JSON string values back into <see cref="Name"/> instances.
/// </remarks>
/// <threadsafety>
/// This class is thread-safe, assuming no shared state outside the scope of the instance is modified.
/// </threadsafety>
/// <seealso cref="Name"/>
/// <seealso cref="JsonConverter{T}"/>
public class NameJsonConverter<T> : JsonConverter<T>
    where T : struct, IName<T>
{
    /// <inheritdoc />
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            var foundString = reader.GetString();
            return foundString is not null
                ? T.FromString(foundString)
                : throw new JsonException("Name cannot be null.");
        }
        catch (InvalidOperationException ex)
        {
            throw new JsonException(ex.Message, ex);
        }
    }

    /// <inheritdoc />
    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Read(ref reader, typeToConvert, options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
    
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.ToString().RequireNonNull());
    }
}
