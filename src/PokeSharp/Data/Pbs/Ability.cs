using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

/// <summary>
/// Represents an ability in the game with a unique identifier, name, description, and associated flags.
/// </summary>
[GameDataEntity(DataPath = "abilities")]
[MessagePackObject(true)]
public partial record Ability
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// The name of the ability, as seen by the player.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// The ability's description.
    /// </summary>
    public required Text Description { get; init; }

    /// <summary>
    /// Comma-separated labels applied to the ability which can be used to make it behave differently. The existing flags are:
    /// - FasterEggHatching - Eggs hatch twice as fast when there are 1 or more Pokémon in the party whose species has this flag.
    /// </summary>
    public required ImmutableArray<Name> Flags { get; init; }

    /// <summary>
    /// Determines whether the specified flag is present in the ability's list of flags.
    /// </summary>
    /// <param name="flag">The flag to check for in the list of flags.</param>
    /// <returns>
    /// <c>true</c> if the specified flag is present in the list of flags; otherwise, <c>false</c>.
    /// </returns>
    public bool HasFlag(Name flag) => Flags.Contains(flag);
}
