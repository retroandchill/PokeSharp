using System.Text.Json.Serialization;
using Injectio.Attributes;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[RegisterSingleton(Factory = nameof(Default))]
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FieldDefinition))]
[JsonSerializable(typeof(FieldPath))]
[JsonSerializable(typeof(FieldEdit))]
[JsonSerializable(typeof(IEnumerable<EditorTabOption>))]
public partial class PokeEditJsonSerializerContext : JsonSerializerContext;
