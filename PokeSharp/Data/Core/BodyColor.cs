using System.Collections;
using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Represents a body color entity used in the PokeSharp data framework.
/// This struct is a part of game data entities and is used to define colors
/// associated with various in-game elements.
/// </summary>
/// <remarks>
/// The <see cref="BodyColor"/> struct is a read-only record type, making it immutable once initialized.
/// It includes a unique identifier and a display name, which are essential for its representation.
/// </remarks>
[GameDataEntity]
public readonly partial record struct BodyColor
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Represents a localized name for the corresponding body color.
    /// </summary>
    public required Text Name { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.BodyColor";

    /// <summary>
    /// Registers default values for the <see cref="BodyColor"/> structure.
    /// </summary>
    /// <remarks>
    /// This method initializes a set of predefined <see cref="BodyColor"/> instances
    /// with their associated identifiers and localized names. These values are registered
    /// for use throughout the application.
    /// </remarks>
    public static void AddDefaultValues()
    {
        Register(new BodyColor { Id = "Red", Name = Text.Localized(LocalizationNamespace, "Red", "Red") });

        Register(new BodyColor { Id = "Blue", Name = Text.Localized(LocalizationNamespace, "Blue", "Blue") });

        Register(new BodyColor { Id = "Yellow", Name = Text.Localized(LocalizationNamespace, "Yellow", "Yellow") });

        Register(new BodyColor { Id = "Green", Name = Text.Localized(LocalizationNamespace, "Green", "Green") });

        Register(new BodyColor { Id = "Black", Name = Text.Localized(LocalizationNamespace, "Black", "Black") });

        Register(new BodyColor { Id = "Brown", Name = Text.Localized(LocalizationNamespace, "Brown", "Brown") });

        Register(new BodyColor { Id = "Purple", Name = Text.Localized(LocalizationNamespace, "Purple", "Purple") });

        Register(new BodyColor { Id = "Gray", Name = Text.Localized(LocalizationNamespace, "Gray", "Gray") });

        Register(new BodyColor { Id = "White", Name = Text.Localized(LocalizationNamespace, "White", "White") });

        Register(new BodyColor { Id = "Pink", Name = Text.Localized(LocalizationNamespace, "Pink", "Pink") });
    }
    #endregion
}
