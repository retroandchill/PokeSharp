using System.Text.Json;
using System.Text.Json.Serialization;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Serialization.Json;

public class EvolutionParameterJsonConverter : JsonConverter<EvolutionParameter>
{
    public override EvolutionParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        return reader.TokenType switch
        {
            JsonTokenType.Null => EvolutionParameter.Null,
            JsonTokenType.String => new EvolutionParameter(reader.GetString()!),
            JsonTokenType.Number => new EvolutionParameter(reader.GetInt32()),
            _ => throw new JsonException("Unexpected token when reading EvolutionParameter")
        };
    }

    public override void Write(Utf8JsonWriter writer, EvolutionParameter value, JsonSerializerOptions options)
    {
        value.Match(writer.WriteNumberValue, n => writer.WriteStringValue(n), writer.WriteNullValue);
    }
}