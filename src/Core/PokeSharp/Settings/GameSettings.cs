using System.Collections.Immutable;
using Injectio.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PokeSharp.Core;
using PokeSharp.Trainers;

namespace PokeSharp.Settings;

public readonly record struct RivalName(Name TrainerType, int Variable);

public readonly record struct HiddenMoveBadgeRequirements()
{
    public int Cut { get; init; } = 1;
    public int Flash { get; init; } = 2;
    public int RockSmash { get; init; } = 3;
    public int Surf { get; init; } = 4;
    public int Fly { get; init; } = 5;
    public int Strength { get; init; } = 6;
    public int Dive { get; init; } = 7;
    public int Waterfall { get; init; } = 8;
}

public enum RoamingEncounterType : byte
{
    LandAndWater,
    LandOnly,
    WaterOnly,
    FishingOnly,
    FishingAndWater,
}

public readonly record struct RoamingSpecies(
    Name Species,
    int Level,
    int GameSwitch,
    RoamingEncounterType EncounterType,
    string? BattleMusic = null,
    IReadOnlyDictionary<int, ImmutableArray<int>>? RoamingAreas = null
);

public readonly record struct PokedexName(Text Name, int Region = Pokedex.NationalDex);

public readonly record struct Language(Text DisplayName, string FileName);

/// <summary>
/// Containing details of a graphic to be shown on the region map if appropriate.
/// </summary>
/// <param name="RegionNumber">The numeric ID of the region</param>
/// <param name="GameSwitch">The graphic is shown if this is ON (non-wall maps only)</param>
/// <param name="X">X coordinate of the graphic on the map, in squares.</param>
/// <param name="Y">Y coordinate of the graphic on the map, in squares.</param>
/// <param name="Graphic">Name of the graphic, found in the Graphics/UI/Town Map folder.</param>
/// <param name="AlwaysVisible">The graphic will always (true) or never (false) be shown on a wall map.</param>
public readonly record struct RegionMapExtra(
    int RegionNumber,
    int GameSwitch,
    int X,
    int Y,
    string Graphic,
    bool AlwaysVisible = false
);

/// <summary>
/// Represents a bag pocket in the player's inventory.
/// </summary>
/// <param name="Name">The name of the pocket in the Bag.</param>
/// <param name="Size">The maximum number of slots per pocket (null means infinite number).</param>
/// <param name="AutoSort">
/// Whether each pocket in turn auto-sorts itself by the order items are defined in the PBS file items.txt.
/// </param>
public readonly record struct BagPocket(Text Name, int? Size = null, bool AutoSort = false);

public readonly record struct BadgeBoosts(int Attack, int Defense, int SpecialAttack, int SpecialDefense, int Speed);

/// <summary>
///
/// </summary>
/// <param name="Version">The version of your game. It has to adhere to the MAJOR.MINOR.PATCH format.</param>
/// <param name="MechanicsGeneration">
/// The generation that the battle system follows. Used throughout the battle scripts, and also by some other settings
/// which are used in and out of battle (you can of course change those settings to suit your game).
/// Note that this isn't perfect. Essentials doesn't accurately replicate every single generation's mechanics.
/// It's considered to be good enough. Only generations 5 and later are reasonably supported.</param>
[AutoServiceShortcut(Type = AutoServiceShortcutType.Options)]
public record GameSettings(string Version, int MechanicsGeneration = 8)
{
    public GameSettings()
        : this("1.0.0") { }

    #region The Player and NPCs

    /// <summary>
    /// The maximum amount of money the player can have.
    /// </summary>
    public int MaxMoney { get; init; } = 999_999;

    /// <summary>
    /// The maximum number of Battle Points the player can have.
    /// </summary>
    public int MaxCoins { get; init; } = 99_999;

    /// <summary>
    /// The maximum amount of soot the player can have.
    /// </summary>
    public int MaxBattlePoints { get; init; } = 9_999;

    /// <summary>
    /// The maximum amount of soot the player can have.
    /// </summary>
    public int MaxSoot { get; init; } = 9_999;

    /// <summary>
    /// The maximum length, in characters, that the player's name can be.
    /// </summary>
    public int MaxPlayerNameSize { get; init; } = 10;

    /// <summary>
    /// Containers a trainer type followed by a Game Variable number. If the Variable isn't set to 0, then
    /// all trainers with the associated trainer type will be named as whatever is in that Variable.
    /// </summary>
    public ImmutableArray<RivalName> RivalNames { get; init; } =
    [new("RIVAL1", 12), new("RIVAL2", 12), new("CHAMPION", 12)];

    #endregion

    #region Overworld

    /// <summary>
    /// Whether outdoor maps should be shaded according to the time of day.
    /// </summary>
    public bool TimeShading { get; init; } = true;

    /// <summary>
    /// Whether the reflections of the player/events will ripple horizontally.
    /// </summary>
    public bool AnimateReflections { get; init; } = true;

    /// <summary>
    /// Whether planted berries grow according to Gen 4 mechanics (true) or Gen 3 mechanics (false).
    /// </summary>
    public bool NewBerryPlants { get; init; } = MechanicsGeneration >= 4;

    /// <summary>
    /// Whether fishing automatically hooks the Pokémon (true), or whether there is a reaction test first (false).
    /// </summary>
    public bool FishingAutoHook { get; init; } = false;

    /// <summary>
    /// The ID of the common event that runs when the player starts fishing (runs instead of showing the casting animation).
    /// </summary>
    public int? FishingBeginCommonEvent { get; init; }

    /// <summary>
    /// The ID of the common event that runs when the player stops fishing (runs instead of showing the reeling in animation).
    /// </summary>
    public int? FishingEndCommonEvent { get; init; }

    /// <summary>
    /// The number of steps allowed before a Safari Zone game is over (0=infinite).
    /// </summary>
    public int SafariSteps { get; init; } = 600;

    /// <summary>
    /// The number of seconds a Bug-Catching Contest lasts for (0=infinite).
    /// </summary>
    public int BugContestTime { get; init; } = 20 * 60;

    /// <summary>
    /// Pairs of map IDs, where the location signpost isn't shown when moving from
    /// one of the maps in a pair to the other (and vice versa). Useful for single
    /// long routes/towns that are spread over multiple maps. e.g. [4,5,16,17,42,43] will be map pairs
    /// 4,5 and 16,17 and 42,43. Moving between two maps that have the exact same name won't show the
    /// location signpost anyway, so you don't need to list those maps here.
    /// </summary>
    public ImmutableArray<int> NoSignPosts { get; init; } = [];

    /// <summary>
    /// Whether poisoned Pokémon will lose HP while walking around in the field.
    /// </summary>
    public bool PoisonInField { get; init; } = MechanicsGeneration <= 4;

    /// <summary>
    /// Whether poisoned Pokémon will faint while walking around in the field (true),
    /// or survive the poisoning with 1 HP (false).
    /// </summary>
    public bool PoisonFaintInField { get; init; } = MechanicsGeneration <= 3;

    #endregion

    #region Using moves in the overworld

    /// <summary>
    /// Whether you need at least a certain number of badges to use some hidden
    /// moves in the field (true), or whether you need one specific badge to use
    /// them (false). The amounts/specific badges are defined below.
    /// </summary>
    public bool FieldMovesCountBadges { get; init; } = true;

    /// <summary>
    /// Depending on <see cref="FieldMovesCountBadges"/>, either the number of badges required
    /// to use each hidden move in the field, or the specific badge number required
    /// to use each move. Remember that badge 0 is the first badge, badge 1 is the
    /// second badge, etc.
    /// e.g. To require the second badge, put false and 1.
    ///        To require at least 2 badges, put true and 2.
    /// </summary>
    public HiddenMoveBadgeRequirements BadgeRequirements { get; init; } = new();

    #endregion

    #region Pokémon

    /// <summary>
    /// The maximum level Pokémon can reach.
    /// </summary>
    public int MaxLevel { get; init; } = 100;

    /// <summary>
    /// The level of newly hatched Pokémon.
    /// </summary>
    public int EggLevel { get; init; } = 1;

    /// <summary>
    /// The odds of a newly generated Pokémon being shiny (out of 65536).
    /// </summary>
    public int ShinyPokemonChance { get; init; } = MechanicsGeneration >= 6 ? 16 : 8;

    /// <summary>
    /// Whether super shininess is enabled (uses a different shiny animation).
    /// </summary>
    public bool SuperShiny { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// Whether Pokémon with the "Legendary", "Mythical" or "Ultra Beast" flags will have at least 3 perfect IVs.
    /// </summary>
    public bool LegendariesHaveSomePerfectIVs { get; init; } = MechanicsGeneration >= 6;

    /// <summary>
    /// The odds of a wild Pokémon/bred egg having Pokérus (out of 65536).
    /// </summary>
    public int PokerusChance { get; init; } = 3;

    /// <summary>
    /// Whether IVs and EVs are treated as 0 when calculating a Pokémon's stats.
    /// IVs and EVs still exist, and are used by Hidden Power and some cosmetic
    /// things as normal.
    /// </summary>
    public bool DisableIVsAndEVs { get; init; }

    /// <summary>
    /// Whether the Move Relearner can also teach egg moves that the Pokémon knew
    /// when it hatched and moves that the Pokémon was once taught by a TR. Moves
    /// from the Pokémon's level-up moveset of the same or a lower level than the
    /// Pokémon can always be relearned.
    /// </summary>
    public bool MoveRelearnerCanTeachMoreMoves { get; init; } = MechanicsGeneration >= 6;

    #endregion

    #region Breeding Pokémon and Day Care

    /// <summary>
    /// Whether Pokémon in the Day Care gain Exp for each step the player takes.
    /// This should be true for the Day Care and false for the Pokémon Nursery, both
    /// of which use the same code in Essentials.
    /// </summary>
    public bool DayCarePokemonGainExpFromWalking { get; init; } = MechanicsGeneration <= 6;

    /// <summary>
    /// Whether two Pokémon in the Day Care can learn egg moves from each other if
    /// they are the same species.
    /// </summary>
    public bool DayCarePokemonCanShareEggMoves { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// Whether a bred baby Pokémon can inherit any TM/TR/HM moves from its father.
    /// It can never inherit TM/TR/HM moves from its mother.
    /// </summary>
    public bool BreedingCanInheritMachineMoves { get; init; } = MechanicsGeneration <= 5;

    /// <summary>
    /// Whether a bred baby Pokémon can inherit any TM/TR/HM moves from its father.
    /// It can never inherit TM/TR/HM moves from its mother.
    /// </summary>
    public bool BreedingCanInheritEggMovesFromMother { get; init; } = MechanicsGeneration >= 6;

    #endregion

    #region Roaming Pokémon

    // csharpier-ignore-start
    /// <summary>
    /// A list of maps used by roaming Pokémon. Each map has an array of other maps it can lead to.
    /// </summary>
    public IReadOnlyDictionary<int, ImmutableArray<int>> RoamingAreas { get; init; } =
        new Dictionary<int, ImmutableArray<int>>
        {
            [5]  = [ 21, 28, 31, 39, 41, 44, 47, 66, 69],
            [21] = [5, 28, 31, 39, 41, 44, 47, 66, 69],
            [28] = [5, 21, 31, 39, 41, 44, 47, 66, 69],
            [31] = [5, 21, 28, 39, 41, 44, 47, 66, 69],
            [39] = [5, 21, 28, 31, 41, 44, 47, 66, 69],
            [41] = [5, 21, 28, 31, 39, 44, 47, 66, 69],
            [44] = [5, 21, 28, 31, 39, 41, 47, 66, 69],
            [47] = [5, 21, 28, 31, 39, 41, 44, 66, 69],
            [66] = [5, 21, 28, 31, 39, 41, 44, 47, 69],
            [69] = [5, 21, 28, 31, 39, 41, 44, 47, 66 ]
        };

    public ImmutableArray<RoamingSpecies> RoamingSpecies { get; init; } =
    [
        new("LATIAS", 30, 53, RoamingEncounterType.LandAndWater, "Battle roaming"),
        new("LATIOS", 30, 53, RoamingEncounterType.LandAndWater, "Battle roaming"),
        new("KYOGE", 40, 54, RoamingEncounterType.WaterOnly,
            RoamingAreas: new Dictionary<int, ImmutableArray<int>>
            {
                [2]  = [   21, 31    ],
                [21] = [2,     31, 69],
                [31] = [2, 21,     69],
                [69] = [   21, 31    ]
            }),
        new("ENTEI", 40, 55, RoamingEncounterType.LandOnly)
    ];
    // csharpier-ignore-end

    #endregion

    #region Party and Pokémon Storage

    /// <summary>
    /// The maximum number of Pokémon that can be in the party.
    /// </summary>
    public int MaxPartySize { get; init; } = 6;

    /// <summary>
    /// The number of boxes in Pokémon storage.
    /// </summary>
    public int NumStorageBoxes { get; init; } = 40;

    /// <summary>
    /// Whether putting a Pokémon into Pokémon storage will heal it. If false, they
    /// are healed by the Recover All: Entire Party event command (at Poké Centers).
    /// </summary>
    public bool HealStoredPokemon { get; init; } = MechanicsGeneration <= 7;

    #endregion

    #region Items

    /// <summary>
    /// Whether various HP-healing items heal the amounts they do in Gen 7+ (true)
    /// or in earlier Generations (false).
    /// </summary>
    public bool RebalancedHealingItemAmounts { get; init; } = MechanicsGeneration >= 7;

    /// <summary>
    /// Whether vitamins can add EVs no matter how many that stat already has in it
    /// (true), or whether they can't make that stat's EVs greater than 100 (false).
    /// </summary>
    public bool NoVitaminEVCap { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// Whether Rage Candy Bar acts as a Full Heal (true) or a Potion (false).
    /// </summary>
    public bool RageCandyBarCuresStatusProblems { get; init; } = MechanicsGeneration >= 7;

    /// <summary>
    /// Whether the Black/White Flutes will raise/lower the levels of wild Pokémon
    /// respectively (true), or will lower/raise the wild encounter rate
    /// respectively (false).
    /// </summary>
    public bool FlutesChangeWildEncounterLevels { get; init; } = MechanicsGeneration >= 6;

    /// <summary>
    /// Whether Rare Candy can be used on a Pokémon that is already at its maximum
    /// level if it is able to evolve by level-up (if so, triggers that evolution).
    /// </summary>
    public bool RareCandyUsableAtMaxLevel { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// Whether the player can choose how many of an item to use at once on a
    /// Pokémon. This applies to Exp-changing items (Rare Candy, Exp Candies) and
    /// EV-changing items (vitamins, feathers, EV-lowering berries).
    /// </summary>
    public bool UseMultipleStatItemsAtOnce { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// If a move taught by a TM/HM/TR replaces another move, this setting is
    /// whether the machine's move retains the replaced move's PP (true), or whether
    /// the machine's move has full PP (false).
    /// </summary>
    public bool TaughtMachinesKeepOldPP { get; init; } = MechanicsGeneration == 5;

    /// <summary>
    /// Whether you get 1 Premier Ball for every 10 of any kind of Poké Ball bought
    /// from a Mart at once (true), or 1 Premier Ball for buying 10+ regular Poké
    /// Balls (false).
    /// </summary>
    public bool MoreBonusPremierBalls { get; init; } = MechanicsGeneration >= 8;

    #endregion

    #region Bag

    /// <summary>
    /// Information about each pocket in the player's bag.
    /// </summary>
    public ImmutableArray<BagPocket> BagPockets { get; init; } =
    [
        new(Text.Localized("Bag.Pockets", "Items", "Items")),
        new(Text.Localized("Bag.Pockets", "Medicine", "Medicine")),
        new(Text.Localized("Bag.Pockets", "PokeBalls", "Poké Balls")),
        new(Text.Localized("Bag.Pockets", "TMsHMs", "TMs & HMs"), AutoSort: true),
        new(Text.Localized("Bag.Pockets", "Berries", "Berries")),
        new(Text.Localized("Bag.Pockets", "Mail", "Mail")),
        new(Text.Localized("Bag.Pockets", "BattleItems", "Battle Items")),
        new(Text.Localized("Bag.Pockets", "KeyItems", "Key Items")),
    ];

    /// <summary>
    /// The maximum number of items each slot in the Bag can hold.
    /// </summary>
    public int BagMaxPerSlot { get; init; } = 999;

    #endregion

    #region Pokédex

    /// <summary>
    /// The names of the Pokédex lists, in the order they are defined in the PBS
    /// file "regional_dexes.txt". The last name is for the National Dex and is
    /// added onto the end of this array.
    /// Each entry is either just a name, or is an array containing a name and a
    /// number. If there is a number, it is a region number as defined in
    /// town_map.txt. If there is no number, the number of the region the player is
    /// currently in will be used. The region number determines which Town Map is
    /// shown in the Area page when viewing that Pokédex list.
    /// </summary>
    public ImmutableArray<PokedexName> PokedexNames { get; init; } =
    [
        new(Text.Localized("Pokedex.Names", "Kanto", "Kanto Pokédex"), 0),
        new(Text.Localized("Pokedex.Names", "Johto", "Johto Pokédex"), 1),
        new(Text.Localized("Pokedex.Names", "National", "National Pokédex")),
    ];

    /// <summary>
    /// Whether the Pokédex list shown is the one for the player's current region
    /// (true), or whether a menu pops up for the player to manually choose which
    /// Dex list to view if more than one is available (false).
    /// </summary>
    public bool UseCurrentRegionDex { get; init; }

    /// <summary>
    /// Whether all forms of a given species will be immediately available to view
    /// in the Pokédex so long as that species has been seen at all (true), or
    /// whether each form needs to be seen specifically before that form appears in
    /// the Pokédex (false).
    /// </summary>
    public bool DexShowsAllForms { get; init; }

    /// <summary>
    /// An array of numbers, where each number is that of a Dex list (in the same
    /// order as above, except the National Dex is -1). All Dex lists included here
    /// will begin their numbering at 0 rather than 1 (e.g. Victini in Unova's Dex).
    /// </summary>
    public ImmutableArray<int> DexesWithOffsets { get; init; } = [];

    /// <summary>
    /// Whether the Pokédex entry of a newly owned species will be shown after it
    /// hatches from an egg, after it evolves and after obtaining it from a trade,
    /// in addition to after catching it in battle.
    /// </summary>
    public bool ShowNewSpeciesPokedexEntryMoreOften { get; init; } = MechanicsGeneration >= 7;

    #endregion

    #region Town Map

    /// <summary>
    /// A set of structs, each containing details of a graphic to be shown on the
    /// region map if appropriate.
    /// </summary>
    public ImmutableArray<RegionMapExtra> RegionMapExtras { get; init; } =
    [new(0, 51, 16, 15, "hidden_Berth"), new(1, 52, 20, 14, "hidden_Faraday")];

    /// <summary>
    /// Whether the player can use Fly while looking at the Town Map. This is only
    /// allowed if the player can use Fly normally.
    /// </summary>
    public bool CanFlyFromTownMap { get; init; } = true;

    #endregion

    #region Phone

    /// <summary>
    /// The default setting for Phone.rematches_enabled, which determines whether
    /// trainers registered in the Phone can become ready for a rematch. If false,
    /// Phone.rematches_enabled = true will enable rematches at any point you want.
    /// </summary>
    public bool PhoneRematchesPossibleFromBeginning { get; init; } = false;

    /// <summary>
    /// Whether the messages in a phone call with a trainer are colored blue or red
    /// depending on that trainer's gender. Note that this doesn't apply to contacts
    /// whose phone calls are in a Common Event; they will need to be colored
    /// manually in their Common Events.
    /// </summary>
    public bool ColorPhoneCallMessagesByContactGender { get; init; } = true;

    #endregion

    #region Battle starting

    /// <summary>
    /// Whether Repel uses the level of the first Pokémon in the party regardless of its HP (true), or it uses the
    /// level of the first unfainted Pokémon (false).
    /// </summary>
    public bool RepelCountsFaintedPokemon { get; init; } = MechanicsGeneration >= 6;

    /// <summary>
    /// Whether more abilities affect whether wild Pokémon appear, which Pokémon they are, etc.
    /// </summary>
    public bool MoreAbilitiesAffectWildEncounters { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// Whether shiny wild Pokémon are more likely to appear if the player has previously defeated/caught lots of
    /// other Pokémon of the same species.
    /// </summary>
    public bool HigherShinyChancesWithNumberBattled { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// Whether overworld weather can set the default terrain effect in battle.
    /// Storm weather sets Electric Terrain, and fog weather sets Misty Terrain.
    /// </summary>
    public bool OverworldWeatherSetsBattleTerrain { get; init; } = MechanicsGeneration >= 8;

    #endregion

    #region Game Switches

    /// <summary>
    /// The Game Switch that is set to ON when the player blacks out.
    /// </summary>
    public int StartingOverSwitch { get; init; } = 1;

    /// <summary>
    /// The Game Switch that is set to ON when the player has seen Pokérus in the
    /// Poké Center (and doesn't need to be told about it again).
    /// </summary>
    public int SeenPokerusSwitch { get; init; } = 2;

    /// <summary>
    /// The Game Switch which, while ON, makes all wild Pokémon created be shiny.
    /// </summary>
    public int ShinyWildPokemonSwitch { get; init; } = 31;

    /// <summary>
    /// The Game Switch which, while ON, makes all Pokémon created considered to be
    /// met via a fateful encounter.
    /// </summary>
    public int FatefulEncounterSwitch { get; init; } = 32;

    /// <summary>
    /// The Game Switch which, while ON, disables the effect of the Pokémon Box Link
    /// and prevents the player from accessing Pokémon storage via the party screen
    /// with it.
    /// </summary>
    public int DisableBoxLinkSwitch { get; init; } = 35;

    #endregion

    #region Overworld animation IDs

    /// <summary>
    /// ID of the animation played when the player steps on grass (grass rustling).
    /// </summary>
    public int GrassAnimationId { get; init; } = 1;

    /// <summary>
    /// ID of the animation played when the player lands on the ground after hopping over a ledge (shows a dust impact).
    /// </summary>
    public int DustAnimationId { get; init; } = 2;

    /// <summary>
    /// ID of the animation played when a trainer notices the player (an exclamation bubble).
    /// </summary>
    public int ExclamationAnimationId { get; init; } = 3;

    /// <summary>
    /// ID of the animation played when a patch of grass rustles due to using the Poké Radar.
    /// </summary>
    public int RustleNormalAnimationId { get; init; } = 1;

    /// <summary>
    /// ID of the animation played when a patch of grass rustles vigorously due to
    /// using the Poké Radar. (Rarer species)
    /// </summary>
    public int RustleVigorousAnimationId { get; init; } = 5;

    /// <summary>
    /// ID of the animation played when a patch of grass rustles and shines due to
    /// using the Poké Radar. (Shiny encounter)
    /// </summary>
    public int RustleShinyAnimationId { get; init; } = 6;

    /// <summary>
    /// ID of the animation played when a berry tree grows a stage while the player
    /// is on the map (for new plant growth mechanics only).
    /// </summary>
    public int PlantSparkleAnimationId { get; init; } = 7;

    #endregion

    #region Messages

    /// <summary>
    /// Available speech frames. These are graphic files in "Graphics/Windowskins/".
    /// </summary>
    public ImmutableArray<string> SpeechWindowskins { get; init; } =
    [
        "speech hgss 1",
        "speech hgss 2",
        "speech hgss 3",
        "speech hgss 4",
        "speech hgss 5",
        "speech hgss 6",
        "speech hgss 7",
        "speech hgss 8",
        "speech hgss 9",
        "speech hgss 10",
        "speech hgss 11",
        "speech hgss 12",
        "speech hgss 13",
        "speech hgss 14",
        "speech hgss 15",
        "speech hgss 16",
        "speech hgss 17",
        "speech hgss 18",
        "speech hgss 19",
        "speech hgss 20",
        "speech pl 18",
    ];

    /// <summary>
    /// Available menu frames. These are graphic files in "Graphics/Windowskins/".
    /// </summary>
    public ImmutableArray<string> MenuWindowskins { get; init; } =
    [
        "choice 1",
        "choice 2",
        "choice 3",
        "choice 4",
        "choice 5",
        "choice 6",
        "choice 7",
        "choice 8",
        "choice 9",
        "choice 10",
        "choice 11",
        "choice 12",
        "choice 13",
        "choice 14",
        "choice 15",
        "choice 16",
        "choice 17",
        "choice 18",
        "choice 19",
        "choice 20",
        "choice 21",
        "choice 22",
        "choice 23",
        "choice 24",
        "choice 25",
        "choice 26",
        "choice 27",
        "choice 28",
    ];

    #endregion

    #region Turn order and disobediance

    /// <summary>
    /// Whether turn order is recalculated after a Pokémon Mega Evolves.
    /// </summary>
    public bool RecalculateTurnOrderAfterMegaEvolution { get; init; } = MechanicsGeneration >= 7;

    /// <summary>
    /// Whether turn order is recalculated after a Pokémon's Speed stat changes.
    /// </summary>
    public bool RecalculateTurnOrderAfterSpeedChanges { get; init; } = MechanicsGeneration >= 8;

    /// <summary>
    /// Whether any Pokémon (originally owned by the player or foreign) can disobey
    /// the player's commands if the Pokémon is too high a level compared to the
    /// number of Gym Badges the player has.
    /// </summary>
    public bool AnyHighLevelPokemonCanDisobey { get; init; } = false;

    /// <summary>
    /// Whether foreign Pokémon can disobey the player's commands if the Pokémon is
    /// too high a level compared to the number of Gym Badges the player has.
    /// </summary>
    public bool ForeignHighLevelPokemonCanDisobey { get; init; } = true;

    #endregion

    #region Mega Evolution

    /// <summary>
    /// The Game Switch which, while ON, prevents all Pokémon in battle from Mega
    /// Evolving even if they otherwise could.
    /// </summary>
    public int NoMegaEvolution { get; init; } = 34;

    #endregion

    #region Move usage calculations

    /// <summary>
    /// Whether a move's physical/special category depends on the move itself as in
    /// newer Gens (true), or on its type as in older Gens (false).
    /// </summary>
    public bool MoveCategoryPerMove { get; init; } = MechanicsGeneration >= 4;

    /// <summary>
    /// Whether critical hits do 1.5x damage and have 4 stages (true), or they do 2x
    /// damage and have 5 stages as in Gen 5 (false). Also determines whether
    /// critical hit rate can be copied by Transform/Psych Up.
    /// </summary>
    public bool NewCriticalHitRateMechanics { get; init; } = MechanicsGeneration >= 6;

    /// <summary>
    /// # Whether several effects apply relating to a Pokémon's type:<br/>
    ///   * Electric-type immunity to paralysis <br/>
    ///   * Ghost-type immunity to being trapped <br/>
    ///   * Grass-type immunity to powder moves and Effect Spore <br/>
    ///   * Poison-type Pokémon can't miss when using Toxic
    /// </summary>
    public bool MoreTypeEffects { get; init; } = MechanicsGeneration >= 6;

    /// <summary>
    /// The minimum number of Gym Badges required to boost each stat of a player's Pokémon by 1.1x, in battle only.
    /// </summary>
    public BadgeBoosts BadgesBoosts { get; init; } =
        new(
            MechanicsGeneration >= 4 ? 999 : 1,
            MechanicsGeneration >= 4 ? 999 : 5,
            MechanicsGeneration >= 4 ? 999 : 7,
            MechanicsGeneration >= 4 ? 999 : 7,
            MechanicsGeneration >= 4 ? 999 : 3
        );

    #endregion

    #region Ability and item effects

    /// <summary>
    /// Whether weather caused by an ability lasts 5 rounds (true) or forever (false).
    /// </summary>
    public bool FixedDurationWeatherFromAbility { get; init; } = MechanicsGeneration >= 6;

    /// <summary>
    /// Whether X items (X Attack, etc.) raise their stat by 2 stages (true) or 1 (false).
    /// </summary>
    public bool XStatItemsRaiseByTwoStages { get; init; } = MechanicsGeneration >= 7;

    /// <summary>
    /// Whether some Poké Balls have catch rate multipliers from Gen 7 (true) or
    /// from earlier generations (false).
    /// </summary>
    public bool NewPokeBallCatchRates { get; init; } = MechanicsGeneration >= 7;

    /// <summary>
    /// Whether Soul Dew powers up Psychic and Dragon-type moves by 20% (true) or
    /// raises the holder's Special Attack and Special Defense by 50% (false).
    /// </summary>
    public bool SoulDewPowersUpTypes { get; init; } = MechanicsGeneration >= 7;

    #endregion

    #region Affection

    /// <summary>
    /// Whether Pokémon with high happiness will gain more Exp from battles, have a
    /// chance of avoiding/curing negative effects by themselves, resisting
    /// fainting, etc.
    /// </summary>
    public bool AffectionEffects
    {
        get;
        init
        {
            field = value;
            ApplyHappinessSoftCap = field;
        }
    }

    /// <summary>
    /// Whether a Pokémon's happiness is limited to 179, and can only be increased
    /// further with friendship-raising berries. Related to AFFECTION_EFFECTS by
    /// default because affection effects only start applying above a happiness of
    /// 179. Also lowers the happiness evolution threshold to 160.
    /// </summary>
    public bool ApplyHappinessSoftCap { get; init; }

    #endregion

    #region Capturing Pokémon

    /// <summary>
    /// Whether the critical capture mechanic applies. Note that its calculation is
    /// based on a total of 600+ species (i.e. that many species need to be caught
    /// to provide the greatest critical capture chance of 2.5x), and there may be
    /// fewer species in your game.
    /// </summary>
    public bool EnableCriticalCaptures { get; init; } = MechanicsGeneration >= 5;

    /// <summary>
    /// Whether the player is asked what to do with a newly caught Pokémon if their
    /// party is full. If true, the player can toggle whether they are asked this in
    /// the Options screen.
    /// </summary>
    public bool NewCaptureCanReplacePartyMember { get; init; } = MechanicsGeneration >= 7;

    #endregion

    #region Exp and EV gain

    /// <summary>
    /// Whether the Exp gained from beating a Pokémon should be scaled depending on the gainer's level.
    /// </summary>
    public bool ScaledExpFormula { get; init; } = MechanicsGeneration is 5 or >= 7;

    /// <summary>
    /// Whether the Exp gained from beating a Pokémon should be divided equally
    /// between each participant (true), or whether each participant should gain
    /// that much Exp (false). This also applies to Exp gained via the Exp Share
    /// (held item version) being distributed to all Exp Share holders.
    /// </summary>
    public bool SplitExpBetweenGainers { get; init; } = MechanicsGeneration <= 5;

    /// <summary>
    /// Whether the Exp gained from beating a Pokémon is multiplied by 1.5 if that
    /// Pokémon is owned by another trainer.
    /// </summary>
    public bool MoreExpFromTrainerPokemon { get; init; } = MechanicsGeneration <= 6;

    /// <summary>
    /// Whether a Pokémon holding a Power item gains 8 (true) or 4 (false) EVs in
    /// the relevant stat.
    /// </summary>
    public bool MoreEVsFromPowerItems { get; init; } = MechanicsGeneration >= 7;

    /// <summary>
    /// Whether Pokémon gain Exp for capturing a Pokémon.
    /// </summary>
    public bool GainExpForCapture { get; init; } = MechanicsGeneration >= 6;

    #endregion

    #region End of battle

    /// <summary>
    /// The Game Switch which, while ON, prevents the player from losing money if
    /// they lose a battle (they can still gain money from trainers for winning).
    /// </summary>
    public int NoMoneyLoss { get; init; } = 33;

    /// <summary>
    /// Whether party Pokémon check whether they can evolve after all battles
    /// regardless of the outcome (true), or only after battles the player won (false).
    /// </summary>
    public bool CheckEvolutionAfterAllBattles { get; init; } = MechanicsGeneration >= 6;

    /// <summary>
    /// Whether fainted Pokémon can try to evolve after a battle.
    /// </summary>
    public bool CheckEvolutionForFaintedPokemon { get; init; } = true;

    #endregion

    #region AI

    /// <summary>
    /// Whether wild Pokémon with the "Legendary", "Mythical" or "UltraBeast" flag
    /// (as defined in pokemon.txt) have a smarter AI. Their skill level is set to
    /// 32, which is a medium skill level.
    /// </summary>
    public bool SmarterWildLegendaryPokemon { get; init; } = true;

    #endregion

    public int StartMoney { get; init; } = 3000;
}
