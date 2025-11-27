using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Model;

[EditableType]
public sealed partial class EditablePokemonType
{
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the display name of the Pokémon type.
    /// </summary>
    public required Text Name { get; set; }

    /// <summary>
    /// The position of the icon.
    /// </summary>
    public required int IconPosition { get; set; }

    /// <summary>
    /// Gets a value indicating whether this type is classified as a Special type.
    /// Special types typically correspond to moves that deal damage from a distance.
    /// </summary>
    public required bool IsSpecialType { get; set; }

    /// <summary>
    /// Gets a value indicating whether this type is considered a pseudo-type.
    /// Pseudo-types are special classifications that don't function as regular types
    /// but may have special meaning in certain game mechanics.
    /// </summary>
    public required bool IsPseudoType { get; set; }

    /// <summary>
    /// Gets the list of types that this type is weak against.
    /// Attacks of these types will deal double damage to Pokémon of this type.
    /// </summary>
    public required List<Name> Weaknesses { get; set; }

    /// <summary>
    /// Gets the list of types that this type is resistant to.
    /// Attacks of these types will deal half damage to Pokémon of this type.
    /// </summary>
    public required List<Name> Resistances { get; set; }

    /// <summary>
    /// Gets the list of types that this type is immune to.
    /// Attacks of these types will deal no damage to Pokémon of this type.
    /// </summary>
    public required List<Name> Immunities { get; set; }

    /// <summary>
    /// A comma-separated list of labels applied to the type which can be used to make it behave differently.
    /// </summary>
    public required List<Name> Flags { get; set; }
}
