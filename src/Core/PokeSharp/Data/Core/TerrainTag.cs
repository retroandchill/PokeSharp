using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.Core.Data;
using PokeSharp.Core.Strings;

namespace PokeSharp.Data.Core;

[GameDataEntity]
public partial record TerrainTag
{
    public required Name Id
    {
        get;
        init
        {
            field = value;
            var fieldString = field.ToString();
            Name = Text.Localized("TerrainTag.Name", fieldString, fieldString);
        }
    }

    public required int IdNumber { get; init; }

    public Text Name { get; init; } = TextConstants.Unnamed;

    public bool CanSurf { get; init; }

    public bool Waterfall { get; init; }

    public bool WaterfallCrest { get; init; }

    public bool CanFish { get; init; }

    public bool CanDive { get; init; }

    public bool DeepBush { get; init; }

    public bool ShowsGrassRustle { get; init; }

    public bool LandWildEncounters { get; init; }

    public bool DoubleWildEncounters { get; init; }

    public Name BattleEnvironment { get; init; }

    public bool Ledge { get; init; }

    public bool Ice { get; init; }

    public bool Bridge { get; init; }

    public bool ShowsReflections { get; init; }

    public bool MustWalk { get; init; }

    public bool MustWalkOrRun { get; init; }

    public bool IgnorePassability { get; init; }
}

[GameDataRegistration<TerrainTag>]
[RegisterSingleton<IGameDataProvider<TerrainTag>>]
public partial class TerrainTagRegistrations
{
    [GameDataEntityRegistration]
    internal static readonly TerrainTag Ledge = new()
    {
        Id = "Ledge",
        IdNumber = 1,
        Ledge = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Grass = new()
    {
        Id = "Grass",
        IdNumber = 2,
        ShowsGrassRustle = true,
        LandWildEncounters = true,
        BattleEnvironment = "Grass",
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Sand = new()
    {
        Id = "Sand",
        IdNumber = 3,
        BattleEnvironment = "Sand",
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Rock = new()
    {
        Id = "Rock",
        IdNumber = 4,
        BattleEnvironment = "Rock",
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag DeepWater = new()
    {
        Id = "DeepWater",
        IdNumber = 5,
        CanSurf = true,
        CanFish = true,
        CanDive = true,
        BattleEnvironment = "MovingWater",
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag StillWater = new()
    {
        Id = "StillWater",
        IdNumber = 6,
        CanSurf = true,
        CanFish = true,
        BattleEnvironment = "StillWater",
        ShowsReflections = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Water = new()
    {
        Id = "Water",
        IdNumber = 7,
        CanSurf = true,
        CanFish = true,
        BattleEnvironment = "MovingWater",
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Waterfall = new()
    {
        Id = "Waterfall",
        IdNumber = 8,
        CanSurf = true,
        Waterfall = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag WaterfallCrest = new()
    {
        Id = "WaterfallCrest",
        IdNumber = 9,
        CanSurf = true,
        CanFish = true,
        WaterfallCrest = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag TallGrass = new()
    {
        Id = "TallGrass",
        IdNumber = 10,
        DeepBush = true,
        LandWildEncounters = true,
        DoubleWildEncounters = true,
        BattleEnvironment = "TallGrass",
        MustWalk = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag UnderwaterGrass = new()
    {
        Id = "UnderwaterGrass",
        IdNumber = 11,
        LandWildEncounters = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Ice = new()
    {
        Id = "Ice",
        IdNumber = 12,
        BattleEnvironment = "Ice",
        Ice = true,
        MustWalkOrRun = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Neutral = new()
    {
        Id = "Neutral",
        IdNumber = 13,
        IgnorePassability = true,
    };

    // NOTE: This is referenced by ID in the "pick_up_soot" proc added to
    //       EventHandlers. It adds soot to the Soot Sack if the player walks over
    //       one of these tiles.

    [GameDataEntityRegistration]
    internal static readonly TerrainTag SootGrass = new()
    {
        Id = "SootGrass",
        IdNumber = 14,
        ShowsGrassRustle = true,
        LandWildEncounters = true,
        BattleEnvironment = "Grass",
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Bridge = new()
    {
        Id = "Bridge",
        IdNumber = 15,
        Bridge = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag Puddle = new()
    {
        Id = "Puddle",
        IdNumber = 16,
        BattleEnvironment = "Puddle",
        ShowsReflections = true,
    };

    [GameDataEntityRegistration]
    internal static readonly TerrainTag NoEffect = new() { Id = "NoEffect", IdNumber = 17 };
}
