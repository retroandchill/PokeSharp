using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Data;

internal interface INameOptionsSource
{
    (int Count, IEnumerable<Name> Names) Options { get; }
}

internal sealed class NameOptionsSource<T> : INameOptionsSource
    where T : IGameDataEntity<Name, T>
{
    public (int Count, IEnumerable<Name> Names) Options => (T.Count, T.Keys);
}
