using System.Collections.Immutable;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

public enum FieldUse : byte
{
    /// <summary>
    /// Not usable in the field
    /// </summary>
    NoFieldUse = 0,

    /// <summary>
    /// Used on a Pokémon
    /// </summary>
    OnPokemon = 1,

    /// <summary>
    /// Used directly from the bag
    /// </summary>
    Direct = 2,

    /// <summary>
    /// Teaches a Pokémon a move (resuable on newer mechanics)
    /// </summary>
    TM = 3,

    /// <summary>
    /// Teaches a Pokémon a move (reusable)
    /// </summary>
    HM = 4,

    /// <summary>
    /// Teaches a Pokémon a move (single-use)
    /// </summary>
    TR = 5,
}

public enum BattleUse : byte
{
    /// <summary>
    /// Not usable in battle
    /// </summary>
    NoBattleUse = 0,

    /// <summary>
    /// Usable on a Pokémon in the party
    /// </summary>
    OnPokemon = 1,

    /// <summary>
    /// Usable on a Pokémon in the party and requiring a move to be selected
    /// </summary>
    OnMove = 2,

    /// <summary>
    /// Usable on the active Pokémon in battle
    /// </summary>
    OnBattler = 3,

    /// <summary>
    /// Used on an opponent in battle
    /// </summary>
    OnFoe = 4,

    /// <summary>
    /// Used directly with no target selection
    /// </summary>
    Direct = 5,
}

[GameDataEntity(DataPath = "items")]
[MessagePackObject(true)]
public partial class Item
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required Text NamePlural { get; init; }

    public required Text PortionName { get; init; }

    public required Text PortionNamePlural { get; init; }

    public required int Pocket { get; init; }

    public required int Price { get; init; }

    public required int SellPrice { get; init; }

    public required int BPPrice { get; init; }

    public required FieldUse FieldUse { get; init; }

    public required BattleUse BattleUse { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public required bool Consumable
    {
        get => !IsImportant && field;
        init;
    }

    public required bool ShowQuantity
    {
        get => field || !IsImportant;
        init;
    }

    public required Name Move { get; init; }

    public required Text Description { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsTM => FieldUse == FieldUse.TM;

    [IgnoreMember]
    [JsonIgnore]
    public bool IsHM => FieldUse == FieldUse.HM;

    [IgnoreMember]
    [JsonIgnore]
    public bool IsTR => FieldUse == FieldUse.TR;

    [IgnoreMember]
    [JsonIgnore]
    public bool IsMachine => IsTM || IsHM || IsTR;

    [IgnoreMember]
    [JsonIgnore]
    public bool IsMail => HasFlag(ItemTags.Mail) || HasFlag(ItemTags.IconMail);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsIconMail => HasFlag(ItemTags.IconMail);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsPokeBall => HasFlag(ItemTags.PokeBall) || HasFlag(ItemTags.SnagBall);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsBerry => HasFlag(ItemTags.Berry);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsKeyItem => HasFlag(ItemTags.KeyItem);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsEvolutionStone => HasFlag(ItemTags.EvolutionStone);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsFossil => HasFlag(ItemTags.Fossil);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsApricorn => HasFlag(ItemTags.Apricorn);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsGem => HasFlag(ItemTags.TypeGem);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsMulch => HasFlag(ItemTags.Mulch);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsMegaStone => HasFlag(ItemTags.MegaStone);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsScent => HasFlag(ItemTags.Scent);

    [IgnoreMember]
    [JsonIgnore]
    public bool IsImportant => IsKeyItem || IsHM || IsTM;

    [IgnoreMember]
    [JsonIgnore]
    public bool CanHold => !IsImportant;

    [IgnoreMember]
    [JsonIgnore]
    public bool ConsumedAfterUse => !IsImportant && Consumable;
}

public static class ItemTags
{
    public static readonly Name Mail = "Mail";
    public static readonly Name IconMail = "IconMail";
    public static readonly Name PokeBall = "PokeBall";
    public static readonly Name SnagBall = "SnagBall";
    public static readonly Name Berry = "Berry";
    public static readonly Name KeyItem = "KeyItem";
    public static readonly Name EvolutionStone = "EvolutionStone";
    public static readonly Name Fossil = "Fossil";
    public static readonly Name Apricorn = "Apricorn";
    public static readonly Name TypeGem = "TypeGem";
    public static readonly Name Mulch = "Mulch";
    public static readonly Name MegaStone = "MegaStone";
    public static readonly Name Scent = "Scent";
}
