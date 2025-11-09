using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Schema;
using PokeSharp.Data;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Model;

[PbsData("moves")]
public record MoveInfo
{
    [PbsSectionName]
    public required Name Id { get; init; }
    
    public Text Name { get; init; } = TextConstants.Unnamed;
    
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(PokemonType), AllowNone = true)]
    public Name Type { get; init; }
    
    public DamageCategory Category { get; init; } = DamageCategory.Status;
    
    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Power { get; init; }
    
    [PbsType(PbsFieldType.UnsignedInteger)]
    public int Accuracy { get; init; } = 100;
    
    [PbsType(PbsFieldType.UnsignedInteger)]
    public int TotalPP { get; init; } = 5;
    
    [PbsType(PbsFieldType.Enumerable, EnumType = typeof(Target), AllowNone = true)]
    public Name Target { get; init; }
    
    public int Priority { get; init; }
    
    public Name FunctionCode { get; init; }

    public IReadOnlySet<string> Flags { get; init; } = ImmutableHashSet<string>.Empty;

    [PbsType(PbsFieldType.UnsignedInteger)]
    public int EffectChance { get; init; }
    
    [PbsType(PbsFieldType.UnformattedText)]
    public Text Description { get; init; } = TextConstants.ThreeQuestions;
}