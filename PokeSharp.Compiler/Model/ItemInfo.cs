using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

[PbsData("items")]
public record ItemInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; init; } = TextConstants.Unnamed;

    public Text NamePlural { get; init; } = TextConstants.Unnamed;

    public Text? PortionName { get; init; }

    public Text? PortionNamePlural { get; init; }
    
    [PbsType(PbsFieldType.PositiveInteger)]
    public int Pocket { get; init; } = 1;
    
    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Price { get; init; }
    
    [PbsType(PbsFieldType.UnsignedInteger)]
    public int? SellPrice { get; init; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int BPPrice { get; init; } = 1;
    
    public FieldUse FieldUse { get; init; } = FieldUse.NoFieldUse;
    
    public BattleUse BattleUse { get; init; } = BattleUse.NoBattleUse;
    
    public IReadOnlySet<string> Flags { get; init; } = ImmutableHashSet<string>.Empty;
    
    public bool? Consumable { get; init; }
    
    public bool? ShowQuantity { get; init; }
    
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move), AllowNone = true)]
    public Name Move { get; init; }
    
    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; init; } = TextConstants.ThreeQuestions;
}