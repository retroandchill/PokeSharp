using System.Collections.Immutable;
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
public partial class Item
{
    public required Name Id { get; init; }
    
    public Text Name { get; init; } = TextConstants.Unnamed;
    
    public Text NamePlural { get; init; } = TextConstants.Unnamed;

    private readonly Text? _portionName;
    public Text PortionName
    {
        get => _portionName ?? Name;
        init => _portionName = value;
    }
    
    private readonly Text? _portionNamePlural;
    public Text PortionNamePlural
    {
        get => _portionNamePlural ?? NamePlural;
        init => _portionNamePlural = value;
    }

    public int Pocket { get; init; } = 1;
    
    public int Price { get; init; } = 0;

    private readonly int? _sellPrice;
    public int SellPrice
    {
        get => _sellPrice ?? Price / 2;
        init => _sellPrice = value;
    }

    public int BPPrice { get; init; } = 1;
    
    public FieldUse FieldUse { get; init; } = FieldUse.NoFieldUse;
    
    public BattleUse BattleUse { get; init; } = BattleUse.NoBattleUse;
    
    public IReadOnlySet<Name> Flags { get; init; } = ImmutableHashSet<Name>.Empty;
    
    public bool IsConsumable { get; init; }

    public bool ShowQuantity
    {
        get => field || !IsImportant;
        init;
        
    } = true;
    
    public Name Move { get; init; }

    public Text Description { get; init; } = TextConstants.ThreeQuestions;
    
    public bool HasFlag(Name flag) => Flags.Contains(flag);
    
    public bool IsTM => FieldUse == FieldUse.TM;
    
    public bool IsHM => FieldUse == FieldUse.HM;
    
    public bool IsTR => FieldUse == FieldUse.TR;
    
    public bool IsMachine => IsTM || IsHM || IsTR;

    public bool IsMail => HasFlag(ItemTags.Mail) || HasFlag(ItemTags.IconMail);
    
    public bool IsIconMail => HasFlag(ItemTags.IconMail);
    
    public bool IsPokeBall => HasFlag(ItemTags.PokeBall) || HasFlag(ItemTags.SnagBall);
    
    public bool IsBerry => HasFlag(ItemTags.Berry);
    
    public bool IsKeyItem => HasFlag(ItemTags.KeyItem);
    
    public bool IsEvolutionStone => HasFlag(ItemTags.EvolutionStone);
    
    public bool IsFossil => HasFlag(ItemTags.Fossil);
    
    public bool IsApricorn => HasFlag(ItemTags.Apricorn);
    
    public bool IsGem => HasFlag(ItemTags.TypeGem);
    
    public bool IsMulch => HasFlag(ItemTags.Mulch);
    
    public bool IsMegaStone => HasFlag(ItemTags.MegaStone);
    
    public bool IsScent => HasFlag(ItemTags.Scent);
    
    public bool IsImportant => IsKeyItem || IsHM || IsTM;

    public bool CanHold => !IsImportant;
    
    public bool ConsumedAfterUse => !IsImportant && IsConsumable;
}

public static class ItemTags
{
    public static readonly Name Mail = "Mail";
    public static readonly Name IconMail = "IconMail";
    public static readonly Name Medicine = "IconMail";
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