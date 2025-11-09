using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;
using ZLinq;

namespace PokeSharp.Data.Pbs;

[GameDataEntity(DataPath = "types")]
public partial record PokemonType
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required int IconPosition { get; init; }

    public bool IsPhysicalType => !IsSpecialType;

    public required bool IsSpecialType { get; init; }

    public required bool IsPseudoType { get; init; }

    public required ImmutableArray<Name> Weaknesses { get; init; }

    public required ImmutableArray<Name> Resistances { get; init; }

    public required ImmutableArray<Name> Immunities { get; init; }

    public required IReadOnlySet<Name> Flags { get; init; }

    public bool HasFlag(Name flag) => Flags.Contains(flag);

    public int GetEffectiveness(Name type)
    {
        if (Weaknesses.Contains(type))
            return Effectiveness.SuperEffective;
        if (Resistances.Contains(type))
            return Effectiveness.NotVeryEffective;

        return Immunities.Contains(type)
            ? Effectiveness.Ineffective
            : Effectiveness.NormalEffective;
    }
}

public static class Effectiveness
{
    public const int Ineffective = 0;
    public const int NotVeryEffective = 1;
    public const int NormalEffective = 2;
    public const int SuperEffective = 4;

    public const float IneffectiveMultiplier = (float)Ineffective / NormalEffective;
    public const float NotVeryEffectiveMultiplier = (float)NotVeryEffective / NormalEffective;
    public const float NormalEffectiveMultiplier = 1.0f;
    public const float SuperEffectiveMultiplier = (float)SuperEffective / NormalEffective;

    private const float Tolerance = 0.0001f;

    public static bool IsIneffective(float effectiveness)
    {
        return Math.Abs(effectiveness - IneffectiveMultiplier) < Tolerance;
    }

    public static bool IsIneffective(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsIneffective(Calculate(attackType, defendTypes));
    }

    public static bool IsResistant(float effectiveness)
    {
        return effectiveness < NormalEffectiveMultiplier;
    }

    public static bool IsResistant(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsResistant(Calculate(attackType, defendTypes));
    }

    public static bool IsNormalEffective(float effectiveness)
    {
        return Math.Abs(effectiveness - NormalEffectiveMultiplier) < Tolerance;
    }

    public static bool IsNormalEffective(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsNormalEffective(Calculate(attackType, defendTypes));
    }

    public static bool IsSuperEffective(float effectiveness)
    {
        return effectiveness > NormalEffectiveMultiplier;
    }

    public static bool IsSuperEffective(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsSuperEffective(Calculate(attackType, defendTypes));
    }

    public static int GetTypeEffectiveness(Name attackType, Name defendType)
    {
        return PokemonType.Get(defendType).GetEffectiveness(attackType);
    }

    public static float Calculate(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return defendTypes
            .AsValueEnumerable()
            .Aggregate(
                NormalEffectiveMultiplier,
                (m, t) => m * GetTypeEffectiveness(attackType, t)
            );
    }
}
