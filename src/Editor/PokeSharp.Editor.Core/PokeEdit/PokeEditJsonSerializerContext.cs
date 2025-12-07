using System.Text.Json.Serialization;
using Injectio.Attributes;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton(Factory = nameof(Default))]
[JsonSourceGenerationOptions(
    WriteIndented = true, 
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    AllowOutOfOrderMetadataProperties = true
)]
[JsonSerializable(typeof(FieldDefinition))]
[JsonSerializable(typeof(FieldPath))]
[JsonSerializable(typeof(FieldEdit))]
[JsonSerializable(typeof(IEnumerable<EditorTabOption>))]
[JsonSerializable(typeof(IEnumerable<Text>))]
[JsonSerializable(typeof(EditorLabelRequest))]
public partial class PokeEditJsonSerializerContext : JsonSerializerContext;
