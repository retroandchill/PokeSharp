using System.Collections.Immutable;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

/// <summary>
/// Specifies how an item can be used in the field.
/// </summary>
public enum FieldUse : byte
{
    /// <summary>
    /// Not usable in the field
    /// </summary>
    NoFieldUse = 0,

    /// <summary>
    /// The item can be used on a Pokémon (e.g. Potions, Elixirs). The party screen will appear when using this item,
    /// allowing you to choose the Pokémon to use it on. Not for TMs, TRs and HMs, though.
    /// </summary>
    OnPokemon = 1,

    /// <summary>
    /// The item can be used out of battle, but it isn't used on a Pokémon (e.g. Repel, Escape Rope, usable Key Items).
    /// </summary>
    Direct = 2,

    /// <summary>
    /// The item is a TM. It teaches a move to a Pokémon, but does not disappear after use.
    /// </summary>
    TM = 3,

    /// <summary>
    /// The item is a HM. It teaches a move to a Pokémon, but does not disappear after use.
    /// Moves taught by a HM cannot be forgotten.
    /// </summary>
    HM = 4,

    /// <summary>
    /// The item is a TR. It teaches a move to a Pokémon, and disappears after use.
    /// </summary>
    TR = 5,
}

/// <summary>
/// Specifies how an item can be used during a battle.
/// </summary>
public enum BattleUse : byte
{
    /// <summary>
    /// Not usable in battle
    /// </summary>
    NoBattleUse = 0,

    /// <summary>
    /// The item can be used on one of your party Pokémon (e.g. Potions, Elixirs, Blue Flute). The party screen will
    /// appear when using this item, allowing you to choose the Pokémon to use it on.
    /// </summary>
    OnPokemon = 1,

    /// <summary>
    /// The item can be used on one of the moves known by one of your party Pokémon (e.g. Ether). The party screen will
    /// appear when using this item, followed by a list of moves to choose from.
    /// </summary>
    OnMove = 2,

    /// <summary>
    /// The item is used on the Pokémon in battle that you are choosing a command for (e.g. X Accuracy, Red/Yellow Flutes).
    /// </summary>
    OnBattler = 3,

    /// <summary>
    /// The item is used on an opposing Pokémon in battle (Poké Balls). If there is more than one opposing Pokémon,
    /// you will be able to choose which of them to use it on.
    /// </summary>
    OnFoe = 4,

    /// <summary>
    /// The item is used with no target (e.g. Poké Doll, Guard Spec., Poké Flute).
    /// </summary>
    Direct = 5,
}

/// <summary>
/// Represents an in-game item with various properties, uses, and flags.
/// </summary>
[GameDataEntity(DataPath = "items")]
[MessagePackObject(true)]
public partial class Item
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// The name of the item, as seen by the player.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// The plural form of the name of the item, as seen by the player. For example, "Potions".
    /// </summary>
    public required Text NamePlural { get; init; }

    /// <summary>
    /// The name of a portion of the item, used in most messages. For example, used to say that the player has found
    /// "1 bag of Stardust" rather than simply "1 Stardust".
    /// </summary>
    public required Text PortionName { get; init; }

    /// <summary>
    /// The plural form of the portion name of the item. For example, "bags of Stardust".
    /// </summary>
    public required Text PortionNamePlural { get; init; }

    /// <summary>
    /// The Bag pocket number the item is stored in.
    /// </summary>
    /// <remarks>
    /// By default, this is one of:
    /// - 1: Items
    /// - 2: Medicine
    /// - 3: Poké Balls
    /// - 4: TMs & HMs
    /// - 5: Berries
    /// - 6: Mail
    /// - 7: Battle Items
    /// - 8: Key Items
    /// </remarks>
    public required int Pocket { get; init; }

    /// <summary>
    /// The cost of the item when the player buys it from a Poké Mart. Note that when the player sells the item to
    /// a Poké Mart, it is sold for half this price (by default; see SellPrice). If an item has a price of 0,
    /// the player cannot sell it.
    /// </summary>
    public required int Price { get; init; }

    /// <summary>
    /// The amount of money gained when selling the item to a Poké Mart.
    /// </summary>
    public required int SellPrice { get; init; }

    /// <summary>
    /// The cost of the item in Battle Points (BP). Note that the player cannot sell items for BP, so there is no
    /// corresponding sell price property.
    /// </summary>
    public required int BPPrice { get; init; }

    /// <summary>
    /// How the item can be used outside of battle.
    /// </summary>
    public required FieldUse FieldUse { get; init; }

    /// <summary>
    /// How the item can be used in battle.
    /// </summary>
    public required BattleUse BattleUse { get; init; }

    /// <summary>
    /// Comma-separated labels applied to the item which can be used to make it behave differently.
    /// </summary>
    public required ImmutableArray<Name> Flags { get; init; }

    /// <summary>
    /// 	Whether the item will be consumed after being used from the Bag. Is true or false. Usually only used to
    /// make a non-Key Item infinite use (e.g. Black/White Flutes).
    /// </summary>
    public required bool Consumable
    {
        get => !IsImportant && field;
        init;
    }

    /// <summary>
    /// Whether the item will show its quantity in the Bag. Only used to show the quantity of a Key Item, TM or HM.
    /// Setting it to false for a different item will not hide its quantity.
    /// </summary>
    public required bool ShowQuantity
    {
        get => field || !IsImportant;
        init;
    }

    /// <summary>
    /// The ID of the move that this item teaches. For HMs, TMs and TRs only.
    /// </summary>
    /// <remarks>
    /// A Pokémon is compatible with a HM/TM/TR only if the move it teaches is listed a a Tutor Move for its species,
    /// or if that move is in its level-up moveset or egg moves.
    /// </remarks>
    public required Name Move { get; init; }

    /// <summary>
    /// The item's description. Typically, HMs, TMs and TRs will have the same description as the moves they teach,
    /// but they don't need to.
    /// </summary>
    public required Text Description { get; init; }

    /// <summary>
    /// Determines whether the specified flag is present in the list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);

    /// <summary>
    /// Indicates whether the item is a Technical Machine (TM).
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsTM => FieldUse == FieldUse.TM;

    /// <summary>
    /// Indicates whether the item is a Hidden Machine (HM).
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsHM => FieldUse == FieldUse.HM;

    /// <summary>
    /// Indicates whether the item is a Technical Record (TR).
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsTR => FieldUse == FieldUse.TR;

    /// <summary>
    /// Indicates whether the item is a Technical Machine, Hidden Machine, or Technical Record.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsMachine => IsTM || IsHM || IsTR;

    /// <summary>
    /// Indicates whether the item is a Mail item.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsMail => HasFlag(ItemTags.Mail) || HasFlag(ItemTags.IconMail);

    /// <summary>
    /// Indicates whether the item is a Mail item, and the images of the holder and two other party Pokémon appear on the Mail.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsIconMail => HasFlag(ItemTags.IconMail);

    /// <summary>
    /// Indicates whether the item is a Poké Ball item.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsPokeBall => HasFlag(ItemTags.PokeBall) || HasFlag(ItemTags.SnagBall);

    /// <summary>
    /// Indicates whether the item is a Snag Ball (i.e. it can capture enemy trainers' Shadow Pokémon).
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsSnagBall => HasFlag(ItemTags.SnagBall);

    /// <summary>
    /// Indicates whether the item is a berry that can be planted.
    /// </summary>
    /// <seealso cref="BerryPlant"/>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsBerry => HasFlag(ItemTags.Berry);

    /// <summary>
    /// Indicates whether the item is a Key Item.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsKeyItem => HasFlag(ItemTags.KeyItem);

    /// <summary>
    /// Indicates whether the item is an evolution stone.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsEvolutionStone => HasFlag(ItemTags.EvolutionStone);

    /// <summary>
    /// Indicates whether the item is a fossil that can be revived. Not to be used for the incomplete fossils from Gen 8 which are pieced
    /// together to revive a Pokémon.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsFossil => HasFlag(ItemTags.Fossil);

    /// <summary>
    /// Indicates whether the item is an Apricorn that can be converted into a Poké Ball.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsApricorn => HasFlag(ItemTags.Apricorn);

    /// <summary>
    /// Indicates whether the item is an elemental power-raising Gem.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsGem => HasFlag(ItemTags.TypeGem);

    /// <summary>
    /// Indicates whether the item is mulch that can be spread on berry patches.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsMulch => HasFlag(ItemTags.Mulch);

    /// <summary>
    /// Indicates whether the item is a Mega Stone. This does NOT include the Red/Blue Orbs.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsMegaStone => HasFlag(ItemTags.MegaStone);

    /// <summary>
    /// Indicates whether the item is a scent, used to lower a Shadow Pokémon's Heart Gauge.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsScent => HasFlag(ItemTags.Scent);

    /// <summary>
    /// Indicates whether the item is considered important, and therefore is automatically not consumed after use.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsImportant => IsKeyItem || IsHM || IsTM;

    /// <summary>
    /// Indicates where the item can be held by a Pokémon.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool CanHold => !IsImportant;

    /// <summary>
    /// Indicates whether the item is consumed after use.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool ConsumedAfterUse => !IsImportant && Consumable;
}

/// <summary>
/// A set of commonly used item flags.
/// </summary>
public static class ItemTags
{
    /// <summary>
    /// The item is a Mail item.
    /// </summary>
    public static readonly Name Mail = "Mail";

    /// <summary>
    /// The item is a Mail item, and the images of the holder and two other party Pokémon appear on the Mail.
    /// </summary>
    public static readonly Name IconMail = "IconMail";

    /// <summary>
    ///  The item is a Poké Ball item.
    /// </summary>
    public static readonly Name PokeBall = "PokeBall";

    /// <summary>
    /// The item is a Snag Ball (i.e. it can capture enemy trainers' Shadow Pokémon).
    /// </summary>
    public static readonly Name SnagBall = "SnagBall";

    /// <summary>
    /// The item is a berry that can be planted.
    /// </summary>
    public static readonly Name Berry = "Berry";

    /// <summary>
    /// The item is a Key Item.
    /// </summary>
    public static readonly Name KeyItem = "KeyItem";

    /// <summary>
    /// The item is an evolution stone.
    /// </summary>
    public static readonly Name EvolutionStone = "EvolutionStone";

    /// <summary>
    /// The item is a fossil that can be revived. Not to be used for the incomplete fossils from Gen 8 which are pieced
    /// together to revive a Pokémon.
    /// </summary>
    public static readonly Name Fossil = "Fossil";

    /// <summary>
    /// The item is an Apricorn that can be converted into a Poké Ball.
    /// </summary>
    public static readonly Name Apricorn = "Apricorn";

    /// <summary>
    ///  The item is an elemental power-raising Gem.
    /// </summary>
    public static readonly Name TypeGem = "TypeGem";

    /// <summary>
    ///  The item is mulch that can be spread on berry patches.
    /// </summary>
    public static readonly Name Mulch = "Mulch";

    /// <summary>
    /// The item is a Mega Stone. This does NOT include the Red/Blue Orbs.
    /// </summary>
    public static readonly Name MegaStone = "MegaStone";

    /// <summary>
    /// The item is a scent used to lower a Shadow Pokémon's Heart Gauge.
    /// </summary>
    public static readonly Name Scent = "Scent";
}
