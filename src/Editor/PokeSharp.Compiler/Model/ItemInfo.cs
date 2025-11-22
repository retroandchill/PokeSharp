using System.Collections.Immutable;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;
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
    [PbsWriteValidation(nameof(ValidateBPPrice))]
    public int BPPrice { get; set; } = 1;

    [PbsWriteValidation(nameof(ValidateFieldUse))]
    public FieldUse FieldUse { get; set; } = FieldUse.NoFieldUse;

    [PbsWriteValidation(nameof(ValidateBattleUse))]
    public BattleUse BattleUse { get; set; } = BattleUse.NoBattleUse;

    public List<string> Flags { get; set; } = [];

    [PbsWriteValidation(nameof(ValidateConsumable))]
    public bool? Consumable { get; set; }

    public bool? ShowQuantity { get; set; }

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Move), AllowNone = true)]
    [PbsWriteValidation(nameof(ValidateMove))]
    public Name Move { get; set; }

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; set; } = TextConstants.ThreeQuestions;

    private static bool ValidateBPPrice(int value) => value != 1;

    private static bool ValidateFieldUse(FieldUse value) => value != FieldUse.NoFieldUse;

    private static bool ValidateBattleUse(BattleUse value) => value != BattleUse.NoBattleUse;

    private static bool ValidateMove(Name value) => !value.IsNone;

    private static bool ValidateConsumable(bool value) => true;
}
