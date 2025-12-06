using System.Text.Json.Serialization;
using PokeSharp.RGSS.RPG;

namespace PokeSharp.RGSS.Serialization;

[RegisterSingleton(Factory = nameof(Default))]
[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Metadata,
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower
)]
[JsonSerializable(typeof(Dictionary<int, MapInfo>))]
[JsonSerializable(typeof(MapInfo))]
[JsonSerializable(typeof(Map))]
public partial class RgssJsonSerializerContext : JsonSerializerContext;
