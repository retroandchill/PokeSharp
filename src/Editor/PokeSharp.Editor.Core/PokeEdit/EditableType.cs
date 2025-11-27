using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IEditableType
{
    Name TypeId { get; }
    Type Type { get; }
    IReadOnlyList<IProperty> Properties { get; }

    bool IsInstanceOfType(object? obj);
}
