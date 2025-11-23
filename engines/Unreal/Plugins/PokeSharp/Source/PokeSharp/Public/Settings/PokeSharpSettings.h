// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Engine/DeveloperSettings.h"

#include "PokeSharpSettings.generated.h"

USTRUCT(BlueprintType)
struct FRivalName
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    FName TrainerType;

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    int32 Variable = 0;
};

USTRUCT(BlueprintType)
struct FHiddenMoveBadgeRequirements
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Cut = 1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Flash = 2;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 RockSmash = 3;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Surf = 4;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Fly = 5;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Strength = 6;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Dive = 7;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Waterfall = 8;
};

USTRUCT(BlueprintType, meta = (NullableEnable))
struct FRoamingAreasList
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    TArray<int32> Areas;
};

UENUM()
enum class ERoamingEncounterType : uint8
{
    LandAndWater,
    LandOnly,
    WaterOnly,
    FishingOnly,
    FishingAndWater,
};

USTRUCT(BlueprintType, meta = (NullableEnable))
struct FRoamingSpecies
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    FName Species;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 Level = 5;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 GameSwitch = 1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    ERoamingEncounterType EncounterType = ERoamingEncounterType::LandAndWater;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    TOptional<FString> BattleMusic;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    TMap<int32, FRoamingAreasList> RoamingAreas;
};

USTRUCT(BlueprintType, meta = (NullableEnable))
struct FBagPocket
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    FText Name;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    TOptional<int32> Size;

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    bool AutoSort = false;

    FBagPocket() = default;

    explicit FBagPocket(FText InName) : Name(MoveTemp(InName))
    {
    }

    FBagPocket(FText InName, const int32 InSize, const bool InAutoSort = false)
        : Name(MoveTemp(InName)), Size(InSize), AutoSort(InAutoSort)
    {
    }

    FBagPocket(FText InName, const bool InAutoSort) : Name(MoveTemp(InName)), AutoSort(InAutoSort)
    {
    }
};

USTRUCT(BlueprintType, meta = (NullableEnable))
struct FPokedexName
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    FText Name;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    TOptional<int32> Region;

    FPokedexName() = default;

    explicit FPokedexName(FText InName) : Name(MoveTemp(InName))
    {
    }

    FPokedexName(FText InName, const int32 InRegion) : Name(MoveTemp(InName)), Region(InRegion)
    {
    }
};

USTRUCT(BlueprintType, meta = (NullableEnable))
struct FRegionMapExtra
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 RegionNumber = 0;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 GameSwitch = 1;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 X = 0;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 0, ClampMin = 0))
    int32 Y = 0;

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    FString Graphic;

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    bool AlwaysVisible = false;

    FRegionMapExtra() = default;

    FRegionMapExtra(const int32 InRegionNumber, const int32 InGameSwitch, const int32 InX, const int32 InY,
                    FString InGraphic, const bool InAlwaysVisible = false)
        : RegionNumber(InRegionNumber), GameSwitch(InGameSwitch), X(InX), Y(InY), Graphic(MoveTemp(InGraphic)),
          AlwaysVisible(InAlwaysVisible)
    {
    }
};

USTRUCT(BlueprintType)
struct FBadgeBoosts
{
    GENERATED_BODY()

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 Attack;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 Defense;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 SpecialAttack;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 SpecialDefense;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (UIMin = 1, ClampMin = 1))
    int32 Speed;
};

/**
 *
 */
UCLASS(Config = Game, DefaultConfig, meta = (NullableEnable))
class POKESHARP_API UPokeSharpSettings : public UDeveloperSettings
{
    GENERATED_BODY()

  public:
    UPokeSharpSettings();

    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Presets", meta = (UIMin = 1, ClampMin = 1))
    int32 MechanicsGeneration = 8;

#pragma region The Player and NPCs
    /**
     * The maximum amount of money the player can have.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "The Player and NPCs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 MaxMoney = 999999;

    /**
     * The maximum number of Battle Points the player can have.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "The Player and NPCs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 MaxCoins = 99999;

    /**
     * The maximum amount of soot the player can have.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "The Player and NPCs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 MaxBattlePoints = 9999;

    /**
     * The maximum amount of soot the player can have.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "The Player and NPCs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 MaxSoot = 9999;

    /**
     * The maximum length, in characters, that the player's name can be.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "The Player and NPCs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 MaxPlayerNameSize = 9999;

    /**
     * Containers a trainer type followed by a Game Variable number. If the Variable isn't set to 0, then
     * all trainers with the associated trainer type will be named as whatever is in that Variable.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "The Player and NPCs")
    TArray<FRivalName> RivalNames;
#pragma endregion

#pragma region Overworld
    /**
     * Whether outdoor maps should be shaded according to the time of day.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld")
    bool TimeShading = true;

    /**
     * Whether the reflections of the player/events will ripple horizontally.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld")
    bool AnimateReflections = true;

    /**
     * Whether planted berries grow according to Gen 4 mechanics (true) or Gen 3 mechanics (false).
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld")
    bool NewBerryPlants = true;

    /**
     * Whether fishing automatically hooks the Pokémon (true), or whether there is a reaction test first (false).
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld")
    bool FishingAutoHook = false;

    /**
     * The ID of the common event that runs when the player starts fishing (runs instead of showing the casting
     * animation).
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld", meta = (UIMin = 1, ClampMin = 1))
    TOptional<int32> FishingBeginCommonEvent;

    /**
     * The ID of the common event that runs when the player stops fishing (runs instead of showing the reeling in
     * animation).
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld", meta = (UIMin = 1, ClampMin = 1))
    TOptional<int32> FishingEndCommonEvent;

    /**
     * The number of steps allowed before a Safari Zone game is over (0=infinite).
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld", meta = (UIMin = 0, ClampMin = 0))
    int32 SafariSteps;

    /**
     * The number of seconds a Bug-Catching Contest lasts for (0=infinite).
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld", meta = (UIMin = 0, ClampMin = 0))
    int32 BugContestTime;

    /**
     * Pairs of map IDs, where the location signpost isn't shown when moving from
     * one of the maps in a pair to the other (and vice versa). Useful for single
     * long routes/towns that are spread over multiple maps. e.g. [4,5,16,17,42,43] will be map pairs
     * 4,5 and 16,17 and 42,43. Moving between two maps that have the exact same name won't show the
     * location signpost anyway, so you don't need to list those maps here.
     */
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld", meta = (UIMin = 1, ClampMin = 1))
    TArray<int32> NoSignPosts;

    /// <summary>
    /// Whether poisoned Pokémon will lose HP while walking around in the field.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld")
    bool PoisonInField;

    /// <summary>
    /// Whether poisoned Pokémon will faint while walking around in the field (true),
    /// or survive the poisoning with 1 HP (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld")
    bool PoisonFaintInField;
#pragma endregion

#pragma region Using moves in the overworld

    /// <summary>
    /// Whether you need at least a certain number of badges to use some hidden
    /// moves in the field (true), or whether you need one specific badge to use
    /// them (false). The amounts/specific badges are defined below.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Using moves in the overworld")
    bool FieldMovesCountBadges = true;

    /// <summary>
    /// Depending on <see cref="FieldMovesCountBadges"/>, either the number of badges required
    /// to use each hidden move in the field, or the specific badge number required
    /// to use each move. Remember that badge 0 is the first badge, badge 1 is the
    /// second badge, etc.
    /// e.g. To require the second badge, put false and 1.
    ///        To require at least 2 badges, put true and 2.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Using moves in the overworld")
    FHiddenMoveBadgeRequirements BadgeRequirements;

#pragma endregion

#pragma region Pokémon

    /// <summary>
    /// The maximum level Pokémon can reach.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon", meta = (UIMin = 1, ClampMin = 1))
    int32 MaxLevel = 100;

    /// <summary>
    /// The level of newly hatched Pokémon.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon", meta = (UIMin = 1, ClampMin = 1))
    int32 EggLevel = 1;

    /// <summary>
    /// The odds of a newly generated Pokémon being shiny (out of 65536).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon",
              meta = (UIMin = 1, ClampMin = 1, UIMax = 65536, ClampMax = 65536))
    int32 ShinyPokemonChance;

    /// <summary>
    /// Whether super shininess is enabled (uses a different shiny animation).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon")
    bool SuperShiny;

    /// <summary>
    /// Whether Pokémon with the "Legendary", "Mythical" or "Ultra Beast" flags will have at least 3 perfect IVs.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon")
    bool LegendariesHaveSomePerfectIVs;

    /// <summary>
    /// The odds of a wild Pokémon/bred egg having Pokérus (out of 65536).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon",
              meta = (UIMin = 1, ClampMin = 1, UIMax = 65536, ClampMax = 65536))
    int32 PokerusChance = 3;

    /// <summary>
    /// Whether IVs and EVs are treated as 0 when calculating a Pokémon's stats.
    /// IVs and EVs still exist, and are used by Hidden Power and some cosmetic
    /// things as normal.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon")
    bool DisableIVsAndEVs;

    /// <summary>
    /// Whether the Move Relearner can also teach egg moves that the Pokémon knew
    /// when it hatched and moves that the Pokémon was once taught by a TR. Moves
    /// from the Pokémon's level-up moveset of the same or a lower level than the
    /// Pokémon can always be relearned.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokémon")
    bool MoveRelearnerCanTeachMoreMoves;

#pragma endregion

#pragma region Breeding Pokémon and Day Care

    /// <summary>
    /// Whether Pokémon in the Day Care gain Exp for each step the player takes.
    /// This should be true for the Day Care and false for the Pokémon Nursery, both
    /// of which use the same code in Essentials.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Breeding Pokémon and Day Care")
    bool DayCarePokemonGainExpFromWalking;

    /// <summary>
    /// Whether two Pokémon in the Day Care can learn egg moves from each other if
    /// they are the same species.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Breeding Pokémon and Day Care")
    bool DayCarePokemonCanShareEggMoves;

    /// <summary>
    /// Whether a bred baby Pokémon can inherit any TM/TR/HM moves from its father.
    /// It can never inherit TM/TR/HM moves from its mother.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Breeding Pokémon and Day Care")
    bool BreedingCanInheritMachineMoves;

    /// <summary>
    /// Whether a bred baby Pokémon can inherit any TM/TR/HM moves from its father.
    /// It can never inherit TM/TR/HM moves from its mother.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Breeding Pokémon and Day Care")
    bool BreedingCanInheritEggMovesFromMother;

#pragma endregion

#pragma region Roaming Pokémon

    /// <summary>
    /// A list of maps used by roaming Pokémon. Each map has an array of other maps it can lead to.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Roaming Pokémon",
              meta = (UIMin = 1, ClampMin = 1))
    TMap<int32, FRoamingAreasList> RoamingAreas;

    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Roaming Pokémon")
    TArray<FRoamingSpecies> RoamingSpecies;

#pragma endregion

#pragma region Party and Pokémon Storage

    /// <summary>
    /// The maximum number of Pokémon that can be in the party.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Party and Pokémon Storage",
              meta = (UIMin = 1, ClampMin = 1))
    int32 MaxPartySize = 6;

    /// <summary>
    /// The number of boxes in Pokémon storage.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Party and Pokémon Storage",
              meta = (UIMin = 1, ClampMin = 1))
    int32 NumStorageBoxes = 40;

    /// <summary>
    /// Whether putting a Pokémon into Pokémon storage will heal it. If false, they
    /// are healed by the Recover All: Entire Party event command (at Poké Centers).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Party and Pokémon Storage")
    bool HealStoredPokemon;

#pragma endregion

#pragma region Items

    /// <summary>
    /// Whether various HP-healing items heal the amounts they do in Gen 7+ (true)
    /// or in earlier Generations (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool RebalancedHealingItemAmounts;

    /// <summary>
    /// Whether vitamins can add EVs no matter how many that stat already has in it
    /// (true), or whether they can't make that stat's EVs greater than 100 (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool NoVitaminEVCap;

    /// <summary>
    /// Whether Rage Candy Bar acts as a Full Heal (true) or a Potion (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool RageCandyBarCuresStatusProblems;

    /// <summary>
    /// Whether the Black/White Flutes will raise/lower the levels of wild Pokémon
    /// respectively (true), or will lower/raise the wild encounter rate
    /// respectively (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool FlutesChangeWildEncounterLevels;

    /// <summary>
    /// Whether Rare Candy can be used on a Pokémon that is already at its maximum
    /// level if it is able to evolve by level-up (if so, triggers that evolution).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool RareCandyUsableAtMaxLevel;

    /// <summary>
    /// Whether the player can choose how many of an item to use at once on a
    /// Pokémon. This applies to Exp-changing items (Rare Candy, Exp Candies) and
    /// EV-changing items (vitamins, feathers, EV-lowering berries).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool UseMultipleStatItemsAtOnce;

    /// <summary>
    /// If a move taught by a TM/HM/TR replaces another move, this setting is
    /// whether the machine's move retains the replaced move's PP (true), or whether
    /// the machine's move has full PP (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool TaughtMachinesKeepOldPP;

    /// <summary>
    /// Whether you get 1 Premier Ball for every 10 of any kind of Poké Ball bought
    /// from a Mart at once (true), or 1 Premier Ball for buying 10+ regular Poké
    /// Balls (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Items")
    bool MoreBonusPremierBalls;

#pragma endregion

#pragma region Bag

    /// <summary>
    /// Information about each pocket in the player's bag.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Bag")
    TArray<FBagPocket> BagPockets = {
        FBagPocket(NSLOCTEXT("Bag.Pockets", "Items", "Items")),
        FBagPocket(NSLOCTEXT("Bag.Pockets", "Medicine", "Medicine")),
        FBagPocket(NSLOCTEXT("Bag.Pockets", "PokeBalls", "Poké Balls")),
        FBagPocket(NSLOCTEXT("Bag.Pockets", "TMsHMs", "TMs & HMs"), true),
        FBagPocket(NSLOCTEXT("Bag.Pockets", "Berries", "Berries")),
        FBagPocket(NSLOCTEXT("Bag.Pockets", "Mail", "Mail")),
        FBagPocket(NSLOCTEXT("Bag.Pockets", "BattleItems", "Battle Items")),
        FBagPocket(NSLOCTEXT("Bag.Pockets", "KeyItems", "Key Items")),
    };

    /// <summary>
    /// The maximum number of items each slot in the Bag can hold.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Bag", meta = (UIMin = 1, ClampMin = 1))
    int32 BagMaxPerSlot = 999;

#pragma endregion

#pragma region Pokédex

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
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokédex")
    TArray<FPokedexName> PokedexNames = {FPokedexName(NSLOCTEXT("Pokedex.Names", "Kanto", "Kanto Pokédex"), 0),
                                         FPokedexName(NSLOCTEXT("Pokedex.Names", "Johto", "Johto Pokédex"), 1),
                                         FPokedexName(NSLOCTEXT("Pokedex.Names", "National", "National Pokédex"))};

    /// <summary>
    /// Whether the Pokédex list shown is the one for the player's current region
    /// (true), or whether a menu pops up for the player to manually choose which
    /// Dex list to view if more than one is available (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokédex")
    bool UseCurrentRegionDex = false;

    /// <summary>
    /// Whether all forms of a given species will be immediately available to view
    /// in the Pokédex so long as that species has been seen at all (true), or
    /// whether each form needs to be seen specifically before that form appears in
    /// the Pokédex (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokédex")
    bool DexShowsAllForms = false;

    /// <summary>
    /// An array of numbers, where each number is that of a Dex list (in the same
    /// order as above, except the National Dex is -1). All Dex lists included here
    /// will begin their numbering at 0 rather than 1 (e.g. Victini in Unova's Dex).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokédex", meta = (UIMin = 1, ClampMin = 1))
    TArray<int32> DexesWithOffsets;

    /// <summary>
    /// Whether the Pokédex entry of a newly owned species will be shown after it
    /// hatches from an egg, after it evolves and after obtaining it from a trade,
    /// in addition to after catching it in battle.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Pokédex")
    bool ShowNewSpeciesPokedexEntryMoreOften;

#pragma endregion

#pragma region Town Map

    /// <summary>
    /// A set of structs, each containing details of a graphic to be shown on the
    /// region map if appropriate.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Town Map")
    TArray<FRegionMapExtra> RegionMapExtras = {FRegionMapExtra(0, 51, 16, 15, "hidden_Berth"),
                                               FRegionMapExtra(1, 52, 20, 14, "hidden_Faraday")};

    /// <summary>
    /// Whether the player can use Fly while looking at the Town Map. This is only
    /// allowed if the player can use Fly normally.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Town Map")
    bool CanFlyFromTownMap = true;

#pragma endregion

#pragma region Phone

    /// <summary>
    /// The default setting for Phone.rematches_enabled, which determines whether
    /// trainers registered in the Phone can become ready for a rematch. If false,
    /// Phone.rematches_enabled = true will enable rematches at any point you want.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Phone")
    bool PhoneRematchesPossibleFromBeginning = false;

    /// <summary>
    /// Whether the messages in a phone call with a trainer are colored blue or red
    /// depending on that trainer's gender. Note that this doesn't apply to contacts
    /// whose phone calls are in a Common Event; they will need to be colored
    /// manually in their Common Events.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Phone")
    bool ColorPhoneCallMessagesByContactGender = true;

#pragma endregion

#pragma region Battle starting

    /// <summary>
    /// Whether Repel uses the level of the first Pokémon in the party regardless of its HP (true), or it uses the
    /// level of the first unfainted Pokémon (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Battle starting")
    bool RepelCountsFaintedPokemon;

    /// <summary>
    /// Whether more abilities affect whether wild Pokémon appear, which Pokémon they are, etc.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Battle starting")
    bool MoreAbilitiesAffectWildEncounters;

    /// <summary>
    /// Whether shiny wild Pokémon are more likely to appear if the player has previously defeated/caught lots of
    /// other Pokémon of the same species.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Battle starting")
    bool HigherShinyChancesWithNumberBattled;

    /// <summary>
    /// Whether overworld weather can set the default terrain effect in battle.
    /// Storm weather sets Electric Terrain, and fog weather sets Misty Terrain.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Battle starting")
    bool OverworldWeatherSetsBattleTerrain;

#pragma endregion

#pragma region Game Switches

    /// <summary>
    /// The Game Switch that is set to ON when the player blacks out.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Game Switches", meta = (UIMin = 1, ClampMin = 1))
    int32 StartingOverSwitch = 1;

    /// <summary>
    /// The Game Switch that is set to ON when the player has seen Pokérus in the
    /// Poké Center (and doesn't need to be told about it again).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Game Switches", meta = (UIMin = 1, ClampMin = 1))
    int32 SeenPokerusSwitch = 2;

    /// <summary>
    /// The Game Switch which, while ON, makes all wild Pokémon created be shiny.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Game Switches", meta = (UIMin = 1, ClampMin = 1))
    int32 ShinyWildPokemonSwitch = 31;

    /// <summary>
    /// The Game Switch which, while ON, makes all Pokémon created considered to be
    /// met via a fateful encounter.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Game Switches", meta = (UIMin = 1, ClampMin = 1))
    int32 FatefulEncounterSwitch = 32;

    /// <summary>
    /// The Game Switch which, while ON, disables the effect of the Pokémon Box Link
    /// and prevents the player from accessing Pokémon storage via the party screen
    /// with it.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Game Switches", meta = (UIMin = 1, ClampMin = 1))
    int32 DisableBoxLinkSwitch = 35;

#pragma endregion

#pragma region Overworld animation IDs

    /// <summary>
    /// ID of the animation played when the player steps on grass (grass rustling).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld animation IDs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 GrassAnimationId = 1;

    /// <summary>
    /// ID of the animation played when the player lands on the ground after hopping over a ledge (shows a dust impact).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld animation IDs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 DustAnimationId = 2;

    /// <summary>
    /// ID of the animation played when a trainer notices the player (an exclamation bubble).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld animation IDs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 ExclamationAnimationId = 3;

    /// <summary>
    /// ID of the animation played when a patch of grass rustles due to using the Poké Radar.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld animation IDs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 RustleNormalAnimationId = 1;

    /// <summary>
    /// ID of the animation played when a patch of grass rustles vigorously due to
    /// using the Poké Radar. (Rarer species)
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld animation IDs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 RustleVigorousAnimationId = 5;

    /// <summary>
    /// ID of the animation played when a patch of grass rustles and shines due to
    /// using the Poké Radar. (Shiny encounter)
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld animation IDs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 RustleShinyAnimationId = 6;

    /// <summary>
    /// ID of the animation played when a berry tree grows a stage while the player
    /// is on the map (for new plant growth mechanics only).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Overworld animation IDs",
              meta = (UIMin = 1, ClampMin = 1))
    int32 PlantSparkleAnimationId = 7;

#pragma endregion

#pragma region Messages

    /// <summary>
    /// Available speech frames. These are graphic files in "Graphics/Windowskins/".
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Messages")
    TArray<FString> SpeechWindowskins = {
        "speech hgss 1",  "speech hgss 2",  "speech hgss 3",  "speech hgss 4",  "speech hgss 5",  "speech hgss 6",
        "speech hgss 7",  "speech hgss 8",  "speech hgss 9",  "speech hgss 10", "speech hgss 11", "speech hgss 12",
        "speech hgss 13", "speech hgss 14", "speech hgss 15", "speech hgss 16", "speech hgss 17", "speech hgss 18",
        "speech hgss 19", "speech hgss 20", "speech pl 18",
    };

    /// <summary>
    /// Available menu frames. These are graphic files in "Graphics/Windowskins/".
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Messages")
    TArray<FString> MenuWindowskins = {
        "choice 1",  "choice 2",  "choice 3",  "choice 4",  "choice 5",  "choice 6",  "choice 7",
        "choice 8",  "choice 9",  "choice 10", "choice 11", "choice 12", "choice 13", "choice 14",
        "choice 15", "choice 16", "choice 17", "choice 18", "choice 19", "choice 20", "choice 21",
        "choice 22", "choice 23", "choice 24", "choice 25", "choice 26", "choice 27", "choice 28",
    };

#pragma endregion

#pragma region Turn order and disobediance

    /// <summary>
    /// Whether turn order is recalculated after a Pokémon Mega Evolves.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Turn order and disobediance")
    bool RecalculateTurnOrderAfterMegaEvolution;

    /// <summary>
    /// Whether turn order is recalculated after a Pokémon's Speed stat changes.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Turn order and disobediance")
    bool RecalculateTurnOrderAfterSpeedChanges;

    /// <summary>
    /// Whether any Pokémon (originally owned by the player or foreign) can disobey
    /// the player's commands if the Pokémon is too high a level compared to the
    /// number of Gym Badges the player has.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Turn order and disobediance")
    bool AnyHighLevelPokemonCanDisobey = false;

    /// <summary>
    /// Whether foreign Pokémon can disobey the player's commands if the Pokémon is
    /// too high a level compared to the number of Gym Badges the player has.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Turn order and disobediance")
    bool ForeignHighLevelPokemonCanDisobey = true;

#pragma endregion

#pragma region Mega Evolution

    /// <summary>
    /// The Game Switch which, while ON, prevents all Pokémon in battle from Mega
    /// Evolving even if they otherwise could.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Evolution", meta = (UIMin = 1, ClampMin = 1));
    int32 NoMegaEvolution = 34;

#pragma endregion

#pragma region Move usage calculations

    /// <summary>
    /// Whether a move's physical/special category depends on the move itself as in
    /// newer Gens (true), or on its type as in older Gens (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Move usage calculations")
    bool MoveCategoryPerMove;

    /// <summary>
    /// Whether critical hits do 1.5x damage and have 4 stages (true), or they do 2x
    /// damage and have 5 stages as in Gen 5 (false). Also determines whether
    /// critical hit rate can be copied by Transform/Psych Up.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Move usage calculations")
    bool NewCriticalHitRateMechanics;

    /// <summary>
    /// # Whether several effects apply relating to a Pokémon's type:<br/>
    ///   * Electric-type immunity to paralysis <br/>
    ///   * Ghost-type immunity to being trapped <br/>
    ///   * Grass-type immunity to powder moves and Effect Spore <br/>
    ///   * Poison-type Pokémon can't miss when using Toxic
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Move usage calculations")
    bool MoreTypeEffects;

    /// <summary>
    /// The minimum number of Gym Badges required to boost each stat of a player's Pokémon by 1.1x, in battle only.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Move usage calculations")
    FBadgeBoosts BadgesBoosts;

#pragma endregion

#pragma region Ability and item effects

    /// <summary>
    /// Whether weather caused by an ability lasts 5 rounds (true) or forever (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Ability and item effects")
    bool FixedDurationWeatherFromAbility;

    /// <summary>
    /// Whether X items (X Attack, etc.) raise their stat by 2 stages (true) or 1 (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, DisplayName = "X-Stat Items Raise by Two Stages",
              Category = "Ability and item effects")
    bool XStatItemsRaiseByTwoStages;

    /// <summary>
    /// Whether some Poké Balls have catch rate multipliers from Gen 7 (true) or
    /// from earlier generations (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Ability and item effects")
    bool NewPokeBallCatchRates;

    /// <summary>
    /// Whether Soul Dew powers up Psychic and Dragon-type moves by 20% (true) or
    /// raises the holder's Special Attack and Special Defense by 50% (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Ability and item effects")
    bool SoulDewPowersUpTypes;

#pragma endregion

#pragma region Affection

    /// <summary>
    /// Whether Pokémon with high happiness will gain more Exp from battles, have a
    /// chance of avoiding/curing negative effects by themselves, resisting
    /// fainting, etc.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Affection")
    bool AffectionEffects;

    /// <summary>
    /// Whether a Pokémon's happiness is limited to 179, and can only be increased
    /// further with friendship-raising berries. Related to AFFECTION_EFFECTS by
    /// default because affection effects only start applying above a happiness of
    /// 179. Also lowers the happiness evolution threshold to 160.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Affection")
    bool ApplyHappinessSoftCap;

#pragma endregion

#pragma region Capturing Pokémon

    /// <summary>
    /// Whether the critical capture mechanic applies. Note that its calculation is
    /// based on a total of 600+ species (i.e. that many species need to be caught
    /// to provide the greatest critical capture chance of 2.5x), and there may be
    /// fewer species in your game.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Capturing Pokémon")
    bool EnableCriticalCaptures;

    /// <summary>
    /// Whether the player is asked what to do with a newly caught Pokémon if their
    /// party is full. If true, the player can toggle whether they are asked this in
    /// the Options screen.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Capturing Pokémon")
    bool NewCaptureCanReplacePartyMember;

#pragma endregion

#pragma region Exp and EV gain

    /// <summary>
    /// Whether the Exp gained from beating a Pokémon should be scaled depending on the gainer's level.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Exp and EV gain")
    bool ScaledExpFormula;

    /// <summary>
    /// Whether the Exp gained from beating a Pokémon should be divided equally
    /// between each participant (true), or whether each participant should gain
    /// that much Exp (false). This also applies to Exp gained via the Exp Share
    /// (held item version) being distributed to all Exp Share holders.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Exp and EV gain")
    bool SplitExpBetweenGainers;

    /// <summary>
    /// Whether the Exp gained from beating a Pokémon is multiplied by 1.5 if that
    /// Pokémon is owned by another trainer.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Exp and EV gain")
    bool MoreExpFromTrainerPokemon;

    /// <summary>
    /// Whether a Pokémon holding a Power item gains 8 (true) or 4 (false) EVs in
    /// the relevant stat.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Exp and EV gain")
    bool MoreEVsFromPowerItems;

    /// <summary>
    /// Whether Pokémon gain Exp for capturing a Pokémon.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "Exp and EV gain")
    bool GainExpForCapture;

#pragma endregion

#pragma region End of battle

    /// <summary>
    /// The Game Switch which, while ON, prevents the player from losing money if
    /// they lose a battle (they can still gain money from trainers for winning).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "End of battle",
              meta = (UIMin = 1, ClampMin = 1));
    int32 NoMoneyLoss = 33;

    /// <summary>
    /// Whether party Pokémon check whether they can evolve after all battles
    /// regardless of the outcome (true), or only after battles the player won (false).
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "End of battle")
    bool CheckEvolutionAfterAllBattles;

    /// <summary>
    /// Whether fainted Pokémon can try to evolve after a battle.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "End of battle")
    bool CheckEvolutionForFaintedPokemon = true;

#pragma endregion

#pragma region AI

    /// <summary>
    /// Whether wild Pokémon with the "Legendary", "Mythical" or "UltraBeast" flag
    /// (as defined in pokemon.txt) have a smarter AI. Their skill level is set to
    /// 32, which is a medium skill level.
    /// </summary>
    UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Config, Category = "AI")
    bool SmarterWildLegendaryPokemon = true;

#pragma endregion

  protected:
    void PostInitProperties() override;

#if WITH_EDITOR
    void PostEditChangeProperty(FPropertyChangedEvent &PropertyChangedEvent) override;

    FText GetSectionText() const override;
#endif

  private:
    bool IsPropertyConfiguredInIni(const FProperty &Property) const;

    void SetMechanicsGeneration(bool SetAll = true);
};
