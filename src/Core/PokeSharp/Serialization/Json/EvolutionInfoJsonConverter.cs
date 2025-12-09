using System.Text.Json;
using System.Text.Json.Serialization;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Serialization.Json;

public class EvolutionInfoJsonConverter : JsonConverter<EvolutionInfo>
{
    public override EvolutionInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject for EvolutionInfo.");
        }

        Name? species = null;
        Name? evolutionMethod = null;
        object? parameter = null;
        bool isPrevious = false;

        // We may need to buffer the Parameter token until we know the type
        JsonElement? parameterElement = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected PropertyName in EvolutionInfo.");

            string propertyName = reader.GetString() ?? throw new JsonException("Null property name.");

            reader.Read(); // move to value

            switch (propertyName)
            {
                case nameof(EvolutionInfo.Species):
                    species = JsonSerializer.Deserialize<Name>(ref reader, options);
                    break;

                case nameof(EvolutionInfo.EvolutionMethod):
                    evolutionMethod = JsonSerializer.Deserialize<Name>(ref reader, options);
                    break;

                case nameof(EvolutionInfo.Parameter):
                    // Buffer the JSON for Parameter so we can deserialize later
                    parameterElement = JsonElement.ParseValue(ref reader);
                    break;

                case nameof(EvolutionInfo.IsPrevious):
                    // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
                    isPrevious = reader.TokenType switch
                    {
                        JsonTokenType.True => true,
                        JsonTokenType.False => false,
                        _ => throw new JsonException("IsPrevious must be a boolean."),
                    };
                    break;

                default:
                    // Skip unknown properties
                    reader.Skip();
                    break;
            }
        }

        if (species is null)
            throw new JsonException("Missing Species property for EvolutionInfo.");
        if (evolutionMethod is null)
            throw new JsonException("Missing EvolutionMethod property for EvolutionInfo.");

        // Get the evolution metadata (Parameter type lives here)
        var evolutionMeta = Evolution.Get(evolutionMethod.Value);

        // If there was a parameter JSON, deserialize it using the correct runtime type
        if (parameterElement is not null)
        {
            // Deserialize to the specific parameter type
            parameter = evolutionMeta.Parameter is not null
                ? parameterElement.Value.Deserialize(evolutionMeta.Parameter, options)
                : null;
        }
        else if (evolutionMeta.Parameter is not null)
        {
            throw new JsonException("Missing Parameter property for EvolutionInfo.");
        }

        return new EvolutionInfo(
            Species: species.Value,
            EvolutionMethod: evolutionMethod.Value,
            Parameter: parameter,
            IsPrevious: isPrevious
        );
    }

    public override void Write(Utf8JsonWriter writer, EvolutionInfo value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(EvolutionInfo.Species));
        JsonSerializer.Serialize(writer, value.Species, options);

        writer.WritePropertyName(nameof(EvolutionInfo.EvolutionMethod));
        JsonSerializer.Serialize(writer, value.EvolutionMethod, options);

        writer.WritePropertyName(nameof(EvolutionInfo.Parameter));

        var evolutionMeta = Evolution.Get(value.EvolutionMethod);
        var parameterType = evolutionMeta.Parameter;

        if (value.Parameter is null)
        {
            writer.WriteNullValue();
        }
        else if (parameterType is not null && parameterType.IsInstanceOfType(value.Parameter))
        {
            // Serialize with the known runtime parameter type
            JsonSerializer.Serialize(writer, value.Parameter, parameterType, options);
        }
        else
        {
            throw new JsonException(
                $"Parameter type {value.Parameter.GetType()} does not match expected type {parameterType}."
            );
        }

        writer.WritePropertyName(nameof(EvolutionInfo.IsPrevious));
        writer.WriteBooleanValue(value.IsPrevious);

        writer.WriteEndObject();
    }
}
