using System.Text.Json.Serialization;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(FieldDefinition))]
[JsonSerializable(typeof(FieldPath))]
[JsonSerializable(typeof(FieldEdit))]
[JsonSerializable(typeof(EditorTabOption))]
public partial class PokeEditJsonSerializerContext : JsonSerializerContext;
