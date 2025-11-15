using System.Collections.Immutable;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;
using ZLinq;

namespace PokeSharp.Data.Pbs;

/// <summary>
/// Represents a type definition used in the game data model.
/// </summary>
/// <remarks>
/// This structure encapsulates the attributes and relationships of a game type, including its
/// identification, display information, physical and special type properties, and type interactions
/// such as weaknesses, resistances, and immunities. This struct is integral in defining type-specific
/// gameplay mechanics and metadata.
/// </remarks>
[GameDataEntity(DataPath = "types")]
[MessagePackObject(true)]
public partial record PokemonType
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the display name of the Pokémon type.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// The position of the icon.
    /// </summary>
    public required int IconPosition { get; init; }

    /// <summary>
    /// Gets a value indicating whether this type is classified as a Physical type.
    /// Physical types typically correspond to moves that make direct contact.
    /// </summary>
    [IgnoreMember]
    [JsonIgnore]
    public bool IsPhysicalType => !IsSpecialType;

    /// <summary>
    /// Gets a value indicating whether this type is classified as a Special type.
    /// Special types typically correspond to moves that deal damage from a distance.
    /// </summary>
    public required bool IsSpecialType { get; init; }

    /// <summary>
    /// Gets a value indicating whether this type is considered a pseudo-type.
    /// Pseudo-types are special classifications that don't function as regular types
    /// but may have special meaning in certain game mechanics.
    /// </summary>
    public required bool IsPseudoType { get; init; }

    /// <summary>
    /// Gets the list of types that this type is weak against.
    /// Attacks of these types will deal double damage to Pokémon of this type.
    /// </summary>
    public required ImmutableArray<Name> Weaknesses { get; init; }

    /// <summary>
    /// Gets the list of types that this type is resistant to.
    /// Attacks of these types will deal half damage to Pokémon of this type.
    /// </summary>
    public required ImmutableArray<Name> Resistances { get; init; }

    /// <summary>
    /// Gets the list of types that this type is immune to.
    /// Attacks of these types will deal no damage to Pokémon of this type.
    /// </summary>
    public required ImmutableArray<Name> Immunities { get; init; }

    /// <summary>
    /// A comma-separated list of labels applied to the type which can be used to make it behave differently.
    /// </summary>
    public required ImmutableArray<Name> Flags { get; init; }

    /// <summary>
    /// Determines whether the specified flag is present in the list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);

    /// <summary>
    /// Gets the effectiveness of this type against the specified type.
    /// </summary>
    /// <param name="type">The attacking type.</param>
    /// <returns></returns>
    public int GetEffectiveness(Name type)
    {
        if (Weaknesses.Contains(type))
            return Effectiveness.SuperEffective;
        if (Resistances.Contains(type))
            return Effectiveness.NotVeryEffective;

        return Immunities.Contains(type) ? Effectiveness.Ineffective : Effectiveness.NormalEffective;
    }
}

/// <summary>
/// Contains static data regarding effectiveness.
/// </summary>
public static class Effectiveness
{
    /// <summary>
    /// The move is ineffective against this type.
    /// </summary>
    public const int Ineffective = 0;

    /// <summary>
    /// The relative value of a type that is not very effective against this type.
    /// </summary>
    public const int NotVeryEffective = 1;

    /// <summary>
    /// The baseline value used for effectiveness calculations.
    /// </summary>
    public const int NormalEffective = 2;

    /// <summary>
    /// The relative value of a type that is super effective against this type.
    /// </summary>
    public const int SuperEffective = 4;

    /// <summary>
    /// The actual multiplier for a move that is ineffective.
    /// </summary>
    public const float IneffectiveMultiplier = (float)Ineffective / NormalEffective;

    /// <summary>
    /// The actual multiplier for a move that is not-very-effective.
    /// </summary>
    public const float NotVeryEffectiveMultiplier = (float)NotVeryEffective / NormalEffective;

    /// <summary>
    /// The actual multiplier for a move that is normally effective.
    /// </summary>
    public const float NormalEffectiveMultiplier = 1.0f;

    /// <summary>
    /// The actual multiplier for a move that is super-effective.
    /// </summary>
    public const float SuperEffectiveMultiplier = (float)SuperEffective / NormalEffective;

    private const float Tolerance = 0.0001f;

    /// <summary>
    /// Checks whether the specified effectiveness is ineffective.
    /// </summary>
    /// <param name="effectiveness">The calculated multiplier</param>
    /// <returns>Is the move ineffective?</returns>
    public static bool IsIneffective(float effectiveness)
    {
        return Math.Abs(effectiveness - IneffectiveMultiplier) < Tolerance;
    }

    /// <summary>
    /// Determines whether the effectiveness value derived from the attack and defense types is ineffective.
    /// </summary>
    /// <param name="attackType">The type of the attacking move.</param>
    /// <param name="defendTypes">The types of the defending Pokémon.</param>
    /// <returns>
    /// <c>true</c> if the effectiveness value indicates the attack is ineffective; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsIneffective(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsIneffective(Calculate(attackType, defendTypes));
    }

    /// <summary>
    /// Determines whether the specified effectiveness indicates the move is resistant.
    /// </summary>
    /// <param name="effectiveness">The calculated effectiveness value of a move.</param>
    /// <returns>
    /// <c>true</c> if the effectiveness is less than the value for a normally effective move; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsResistant(float effectiveness)
    {
        return effectiveness < NormalEffectiveMultiplier;
    }

    /// <summary>
    /// Determines whether the defending type is resistant to the attacking type based on calculated effectiveness.
    /// </summary>
    /// <param name="attackType">The type of the attacking entity.</param>
    /// <param name="defendTypes">The types of the defending entities to evaluate resistance against.</param>
    /// <returns>
    /// <c>true</c> if the defending type is resistant to the attacking type; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsResistant(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsResistant(Calculate(attackType, defendTypes));
    }

    /// <summary>
    /// Determines whether the specified effectiveness value corresponds to a normally effective multiplier.
    /// </summary>
    /// <param name="effectiveness">The calculated multiplier to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the specified multiplier matches the normally effective value; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNormalEffective(float effectiveness)
    {
        return Math.Abs(effectiveness - NormalEffectiveMultiplier) < Tolerance;
    }

    /// <summary>
    /// Determines whether the effectiveness of an attack against a set of defending types is considered normal.
    /// </summary>
    /// <param name="attackType">The type of the attacking move.</param>
    /// <param name="defendTypes">The types of the defending Pokémon(s).</param>
    /// <returns>
    /// <c>true</c> if the attack is normally effective against the defending types; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNormalEffective(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsNormalEffective(Calculate(attackType, defendTypes));
    }

    /// <summary>
    /// Determines whether the given effectiveness value indicates a super effective move.
    /// </summary>
    /// <param name="effectiveness">The effectiveness multiplier to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the effectiveness value is greater than the normal effectiveness multiplier; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSuperEffective(float effectiveness)
    {
        return effectiveness > NormalEffectiveMultiplier;
    }

    /// <summary>
    /// Determines whether the specified attack type is super effective against the given defending types.
    /// </summary>
    /// <param name="attackType">The type of the attacking move.</param>
    /// <param name="defendTypes">The defending types to evaluate against the attack type.</param>
    /// <returns>
    /// <c>true</c> if the specified attack type is super effective against any of the defending types; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSuperEffective(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return IsSuperEffective(Calculate(attackType, defendTypes));
    }

    /// <summary>
    /// Determines the effectiveness of an attack type against a defense type.
    /// </summary>
    /// <param name="attackType">The type of the attacking move.</param>
    /// <param name="defendType">The type of the defending target.</param>
    /// <returns>
    /// An integer representing the effectiveness of the attack. Possible values include:
    /// 0 for "Ineffective", 1 for "Not Very Effective", 2 for "Normal Effective", and 4 for "Super Effective".
    /// </returns>
    public static int GetTypeEffectiveness(Name attackType, Name defendType)
    {
        return PokemonType.Get(defendType).GetEffectiveness(attackType);
    }

    /// <summary>
    /// Calculates the combined effectiveness of an attack type against multiple defending types.
    /// </summary>
    /// <param name="attackType">The type of the attacking move.</param>
    /// <param name="defendTypes">A collection of defending types to calculate effectiveness against.</param>
    /// <returns>
    /// A <c>float</c> value representing the cumulative effectiveness multiplier of the attack against the defending types.
    /// </returns>
    public static float Calculate(Name attackType, params ReadOnlySpan<Name> defendTypes)
    {
        return defendTypes
            .AsValueEnumerable()
            .Aggregate(NormalEffectiveMultiplier, (m, t) => m * GetTypeEffectiveness(attackType, t));
    }
}
