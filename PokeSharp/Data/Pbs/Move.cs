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

    public required Text Name { get; init; }

    public required Name Type { get; init; }

    public required DamageCategory Category { get; init; }

    public required int Power { get; init; }

    public required int Accuracy { get; init; }

    public required int TotalPP { get; init; }

    public required Name Target { get; init; }

    public required int Priority { get; init; }

    public required Name FunctionCode { get; init; }

    public required ImmutableArray<Name> Flags { get; init; }

    public required int EffectChance { get; init; }

    public required Text Description { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);

    public bool IsPhysical => Category == DamageCategory.Physical;

    public bool IsSpecial => Category == DamageCategory.Special;

    public bool IsDamaging => Category != DamageCategory.Status;

    public bool IsStatus => Category == DamageCategory.Status;

    public bool IsHiddenMove => Item.Entities.Any(i => i.IsHM && i.Move == Id);
}
