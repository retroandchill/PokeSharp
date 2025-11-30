using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IEntityEditor
{
    public Name Id { get; }
    public Text Name { get; }

    public int Order { get; }

    public TypeDefinition Type { get; }
    public Type ClrType { get; }
}
