using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

[PbsData("items")]
public partial class ItemInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; set; } = TextConstants.Unnamed;

    public Text NamePlural { get; set; } = TextConstants.Unnamed;

    public Text? PortionName { get; set; }

    public Text? PortionNamePlural { get; set; }

    [PbsType(PbsFieldType.PositiveInteger)]
    public int Pocket { get; set; } = 1;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Price { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int? SellPrice { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int BPPrice { get; set; } = 1;

    public FieldUse FieldUse { get; set; } = FieldUse.NoFieldUse;

    public BattleUse BattleUse { get; set; } = BattleUse.NoBattleUse;

    public List<string> Flags { get; set; } = [];

    public bool? Consumable { get; set; }

    public bool? ShowQuantity { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move), AllowNone = true)]
    public Name Move { get; set; }

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; set; } = TextConstants.ThreeQuestions;
}
