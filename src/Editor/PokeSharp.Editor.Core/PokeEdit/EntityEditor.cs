using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IEntityEditor
{
    public TypeDefinition Type { get; }
    public Type ClrType { get; }
}