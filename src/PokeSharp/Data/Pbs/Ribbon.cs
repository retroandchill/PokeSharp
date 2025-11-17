using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

/// <summary>
/// Represents a ribbon, which are awarded to the player's Pokémon for various reasons.
/// </summary>
[GameDataEntity(DataPath = "ribbons")]
[MessagePackObject(true)]
public partial record Ribbon
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// The name of the ribbon, as seen by the player in a Pokémon's summary screen.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// The ribbon's description, as seen by the player in a Pokémon's summary screen.
    /// </summary>
    public required Text Description { get; init; }

    /// <summary>
    /// Is a number that determines which sprite to use from the file Graphics/UI/Summary/ribbons.png as the image of
    /// the ribbon. A value of 0 means the top left sprite, 1 means the second one along in the top row, and so on.
    /// The graphic is exactly 8 ribbon sprites wide.
    /// </summary>
    public required int IconPosition { get; init; }

    /// <summary>
    /// Comma-separated labels applied to the ribbon which can be used to make it behave differently.
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
}
