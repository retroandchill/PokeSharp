using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "regional_dexes")]
public readonly partial record struct RegionalDex(int Id, ImmutableArray<Name> Entries);
