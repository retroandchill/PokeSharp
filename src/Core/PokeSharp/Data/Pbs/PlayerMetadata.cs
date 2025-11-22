using System.Diagnostics.CodeAnalysis;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Data;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "player_metadata")]
[MessagePackObject(true)]
public partial record PlayerMetadata
{
    public required int Id { get; init; }

    public required Name TrainerType { get; init; }

    public required string WalkCharset { get; init; }

    [field: AllowNull]
    public required string RunCharset
    {
        get => field ?? WalkCharset;
        init;
    }

    [field: AllowNull]
    public required string CycleCharset
    {
        get => field ?? RunCharset;
        init;
    }

    [field: AllowNull]
    public required string SurfCharset
    {
        get => field ?? CycleCharset;
        init;
    }

    [field: AllowNull]
    public required string DiveCharset
    {
        get => field ?? SurfCharset;
        init;
    }

    [field: AllowNull]
    public required string FishCharset
    {
        get => field ?? WalkCharset;
        init;
    }

    [field: AllowNull]
    public required string SurfFishCharset
    {
        get => field ?? FishCharset;
        init;
    }

    public required HomeLocation? Home { get; init; }

    public static PlayerMetadata Get() => Get(1);
}
