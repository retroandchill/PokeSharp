using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public readonly record struct EditorLabelRequest(Name EditorId);

public readonly record struct EntityRequest(Name EditorId, int Index);

public readonly record struct EntityUpdateRequest(Name EditorId, int Index, ObjectDiffNode Change);
