using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;
using PokeSharp.Unreal.Core.Strings;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Editor.Data;

internal interface INameOptionsSource
{
    (int Count, IEnumerable<FName> Names) Options { get; }
}

internal sealed class NameOptionsSource<T> : INameOptionsSource
    where T : IGameDataEntity<Name, T>
{
    public (int Count, IEnumerable<FName> Names) Options => (T.Count, T.Keys.Select(x => x.ToUnrealName()));
}
