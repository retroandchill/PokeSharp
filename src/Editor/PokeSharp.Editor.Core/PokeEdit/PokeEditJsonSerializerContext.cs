using System.Text.Json.Serialization;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(FieldDefinition))]
public partial class PokeEditJsonSerializerContext : JsonSerializerContext;