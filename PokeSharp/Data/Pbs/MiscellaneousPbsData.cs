using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[GameDataEntity]
public readonly partial record struct RegionalDex(int Id, ImmutableArray<Name> Entries);
