using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public readonly record struct EntityRequest(Name EditorId, int Index);
