using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableMember
{
    Name Name { get; }
    Text DisplayName { get; }
    Text Tooltip { get; }
    Text Category { get; }

    bool HasMetadata(string key);
    string? GetMetadata(string key);
}