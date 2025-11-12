using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a status effect within the game data.
/// </summary>
/// <remarks>
/// The <c>Status</c> structure encapsulates the information necessary to define a status effect,
/// such as its unique identifier and localized name. This struct is immutable and serves as
/// an integral part of the game's data model.
/// </remarks>
[GameDataEntity]
public readonly partial record struct Status
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets or sets the localized name of the status effect.
    /// </summary>
    /// <remarks>
    /// This property represents the name of the status effect in a localized format. It is used
    /// to display the name of the status within user-facing interfaces or game data where localization
    /// is necessary.
    /// </remarks>
    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Status";

    /// <summary>
    /// Adds default values for status effects to the relevant collection or registry.
    /// </summary>
    /// <remarks>
    /// This method predefines a set of commonly used status effects with their associated localized names
    /// and identifiers, such as "SLEEP," "POISON," "BURN," "PARALYSIS," and "FROZEN."
    /// These status effects are registered for use within the game's data model.
    /// </remarks>
    public static void AddDefaultValues()
    {
        Register(new Status { Id = "SLEEP", Name = Text.Localized(LocalizationNamespace, "SLEEP", "Sleep") });

        Register(new Status { Id = "POISON", Name = Text.Localized(LocalizationNamespace, "POISON", "Poison") });

        Register(new Status { Id = "BURN", Name = Text.Localized(LocalizationNamespace, "BURN", "Burn") });

        Register(
            new Status { Id = "PARALYSIS", Name = Text.Localized(LocalizationNamespace, "PARALYSIS", "Paralysis") }
        );

        Register(new Status { Id = "FROZEN", Name = Text.Localized(LocalizationNamespace, "FROZEN", "Frozen") });
    }
    #endregion
}
