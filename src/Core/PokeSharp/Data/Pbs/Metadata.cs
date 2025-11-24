using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Data.Pbs;

[MessagePackObject(true)]
public readonly record struct HomeLocation(int MapId, int X, int Y, int Direction);

[GameDataEntity(DataPath = "metadata")]
[MessagePackObject(true)]
public partial record Metadata
{
    public required int Id { get; init; }

    public required int StartMoney { get; init; }

    public required ImmutableArray<Name> StartItemStorage { get; init; }

    public required HomeLocation Home { get; init; }

    public required Text StorageCreator { get; init; }

    public required string? WildBattleBGM { get; init; }

    public required string? TrainerBattleBGM { get; init; }

    public required string? WildVictoryBGM { get; init; }

    public required string? TrainerVictoryBGM { get; init; }

    public required string? WildCaptureME { get; init; }

    public required string? SurfBGM { get; init; }

    public required string? BicycleBGM { get; init; }

    public static Metadata Instance => Get(0);
}
