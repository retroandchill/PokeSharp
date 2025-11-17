using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[MessagePackObject(true)]
public readonly record struct EncounterId(int MapId, int Version = 0)
{
    public static implicit operator EncounterId(int mapId) => new(mapId);
}

[MessagePackObject(true)]
public readonly record struct EncounterEntry(int Chance, SpeciesForm Species, int MinLevel, int MaxLevel);

[GameDataEntity(DataPath = "encounters")]
[MessagePackObject(true)]
public partial record Encounter
{
    public required EncounterId Id { get; init; }

    public int MapId => Id.MapId;

    public int Version => Id.Version;

    public required IReadOnlyDictionary<Name, int> StepChances { get; init; }

    public required IReadOnlyDictionary<Name, ImmutableArray<EncounterEntry>> Types { get; init; }

    public static bool Exists(int mapId, int mapVersion = 0) => Exists(new EncounterId(mapId, mapVersion));

    public static Encounter Get(int mapId, int mapVersion = 0) => Get(new EncounterId(mapId, mapVersion));

    public static bool TryGet(int mapId, [NotNullWhen(true)] out Encounter? encounter)
    {
        return TryGet(new EncounterId(mapId), out encounter);
    }

    public static bool TryGet(int mapId, int mapVersion, [NotNullWhen(true)] out Encounter? encounter)
    {
        return TryGet(new EncounterId(mapId, mapVersion), out encounter);
    }

    public static IEnumerable<Encounter> OrderedEntities => Entities.OrderBy(a => a.MapId).ThenBy(a => a.Version);

    public static IEnumerable<Encounter> GetEntitiesWithVersion(int version = 0)
    {
        return Entities.Where(a => a.Version == version).OrderBy(a => a.MapId);
    }
}
