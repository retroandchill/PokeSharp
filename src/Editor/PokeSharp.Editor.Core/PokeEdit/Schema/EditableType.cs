using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IEditableType
{
    Name Name { get; }
    IReadOnlyList<IProperty> Properties { get; }
}

public sealed record EditableType : IEditableType
{
    public required Name Name { get; init; }
    public required IReadOnlyList<IProperty> Properties { get; init; }

    private static int _nextId = 1;

    public static int GetNextId()
    {
        var nextId = _nextId;
        Interlocked.Increment(ref _nextId);
        return nextId;
    }
}
