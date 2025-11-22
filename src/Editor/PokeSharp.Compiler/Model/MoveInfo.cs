using System.Collections.Immutable;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Core;
using PokeSharp.Data;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

[PbsData("moves")]
public partial class MoveInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }

    public Text Name { get; set; } = TextConstants.Unnamed;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType), AllowNone = true)]
    public Name Type { get; set; }

    public DamageCategory Category { get; set; } = DamageCategory.Status;

    [PbsType(PbsFieldType.UnsignedInteger)]
    [PbsWriteValidation(nameof(ValidateNumericValue))]
    public int Power { get; set; }

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Accuracy { get; set; } = 100;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int TotalPP { get; set; } = 5;

    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Target), AllowNone = true)]
    public Name Target { get; set; }

    [PbsWriteValidation(nameof(ValidateNumericValue))]
    public int Priority { get; set; }

    public Name FunctionCode { get; set; }

    public List<string> Flags { get; set; } = [];

    [PbsType(PbsFieldType.UnsignedInteger)]
    [PbsWriteValidation(nameof(ValidateNumericValue))]
    public int EffectChance { get; set; }

    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; set; } = TextConstants.ThreeQuestions;

    private static bool ValidateNumericValue(int value) => value != 0;
}
