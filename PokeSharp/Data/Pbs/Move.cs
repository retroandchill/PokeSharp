using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

public enum DamageCategory : byte
{
    Physical,
    Special,
    Status,
}

[GameDataEntity(DataPath = "moves")]
public partial record Move
{
    public required Name Id { get; init; }
    
    public Text Name { get; init; } = TextConstants.Unnamed;

    public Name Type { get; init; }

    public DamageCategory DamageCategory { get; init; } = DamageCategory.Status;

    public int Power { get; init; } = 0;

    public int Accuracy { get; init; } = 100;

    public int TotalPP { get; init; } = 5;
    
    public Name Target { get; init; }
    
    public int Priority { get; init; } = 0;
    
    public Name FunctionCode { get; init; }
    
    public IReadOnlySet<Name> Flags { get; init; } = ImmutableHashSet<Name>.Empty;
    
    public int EffectChance { get; init; } = 0;
    
    public Text Description { get; init; } = TextConstants.ThreeQuestions;
    
    public bool HasFlag(Name flag) => Flags.Contains(flag);
    
    public bool IsPhysical => DamageCategory == DamageCategory.Physical;
    
    public bool IsSpecial => DamageCategory == DamageCategory.Special;
    
    public bool IsDamaging => DamageCategory != DamageCategory.Status;
    
    public bool IsStatus => DamageCategory == DamageCategory.Status;

    public bool IsHiddenMove => Item.Entities.Any(i => i.IsHM && i.Move == Id);
}