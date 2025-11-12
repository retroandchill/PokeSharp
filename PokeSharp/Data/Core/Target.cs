using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

/// <summary>
/// Specifies the number of targets for an action or effect.
/// </summary>
public enum TargetCount : byte
{
    /// <summary>
    /// The move have no specific target.
    /// </summary>
    NoTargets,

    /// <summary>
    /// The move hits a single target.
    /// </summary>
    SingleTarget,

    /// <summary>
    /// The move hits multiple targets.
    /// </summary>
    MultipleTargets,
}

/// <summary>
/// Represents a target configuration within the PokeSharp framework. A target defines
/// how and where an action, ability, or effect can be applied in the context of the game.
/// </summary>
[GameDataEntity]
public partial record Target
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the name associated with the target.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// The number of targets that the move can affect.
    /// </summary>
    public TargetCount NumTargets { get; init; }

    /// <summary>
    /// Indicates whether the target entity is directed at a foe.
    /// </summary>
    public bool TargetsFoe { get; init; }

    /// <summary>
    /// Indicates whether the target affects all entities.
    /// </summary>
    public bool TargetsAll { get; init; }

    /// <summary>
    /// Determines whether the target affects the foe's side in the game environment.
    /// </summary>
    public bool AffectsFoeSide { get; init; }

    /// <summary>
    /// Represents whether the target has long-range capabilities.
    /// </summary>
    public bool LongRange { get; init; }

    #region Defaults

    private const string LocalizationNamespace = "GameData.Target";

    /// <summary>
    /// Adds default values to the Target data entity. This method is used to
    /// set predefined configuration or properties associated with the Target.
    /// </summary>
    public static void AddDefaultValues()
    {
        Register(new Target { Id = "User", Name = Text.Localized(LocalizationNamespace, "User", "User") });

        Register(
            new Target
            {
                Id = "NearAlly",
                Name = Text.Localized(LocalizationNamespace, "NearAlly", "Near Ally"),
                NumTargets = TargetCount.SingleTarget,
            }
        );

        Register(
            new Target
            {
                Id = "UserOrNearAlly",
                Name = Text.Localized(LocalizationNamespace, "UserOrNearAlly", "User or Near Ally"),
                NumTargets = TargetCount.SingleTarget,
            }
        );

        Register(
            new Target
            {
                Id = "AllAllies",
                Name = Text.Localized(LocalizationNamespace, "AllAllies", "All Allies"),
                NumTargets = TargetCount.MultipleTargets,
                TargetsAll = true,
                LongRange = true,
            }
        );

        Register(
            new Target
            {
                Id = "UserAndAllies",
                Name = Text.Localized(LocalizationNamespace, "UserAndAllies", "User and Allies"),
                NumTargets = TargetCount.MultipleTargets,
                LongRange = true,
            }
        );

        Register(
            new Target
            {
                Id = "NearFoe",
                Name = Text.Localized(LocalizationNamespace, "NearFoe", "Near Foe"),
                NumTargets = TargetCount.SingleTarget,
                TargetsFoe = true,
            }
        );

        Register(
            new Target
            {
                Id = "RandomNearFoe",
                Name = Text.Localized(LocalizationNamespace, "RandomNearFoe", "Random Near Foe"),
                NumTargets = TargetCount.SingleTarget,
                TargetsFoe = true,
            }
        );

        Register(
            new Target
            {
                Id = "AllNearFoes",
                Name = Text.Localized(LocalizationNamespace, "AllNearFoes", "All Near Foes"),
                NumTargets = TargetCount.MultipleTargets,
                TargetsFoe = true,
            }
        );

        Register(
            new Target
            {
                Id = "Foe",
                Name = Text.Localized(LocalizationNamespace, "Foe", "Foe"),
                NumTargets = TargetCount.SingleTarget,
                TargetsFoe = true,
                LongRange = true,
            }
        );

        Register(
            new Target
            {
                Id = "AllFoes",
                Name = Text.Localized(LocalizationNamespace, "AllFoes", "All Foes"),
                NumTargets = TargetCount.MultipleTargets,
                TargetsFoe = true,
                LongRange = true,
            }
        );

        Register(
            new Target
            {
                Id = "NearOther",
                Name = Text.Localized(LocalizationNamespace, "NearOther", "Near Other"),
                NumTargets = TargetCount.SingleTarget,
                TargetsFoe = true,
            }
        );

        Register(
            new Target
            {
                Id = "AllNearOthers",
                Name = Text.Localized(LocalizationNamespace, "AllNearOthers", "All Near Others"),
                NumTargets = TargetCount.MultipleTargets,
                TargetsFoe = true,
            }
        );

        Register(
            new Target
            {
                Id = "Other",
                Name = Text.Localized(LocalizationNamespace, "Other", "Other"),
                NumTargets = TargetCount.SingleTarget,
                TargetsFoe = true,
                LongRange = true,
            }
        );

        Register(
            new Target
            {
                Id = "AllBattlers",
                Name = Text.Localized(LocalizationNamespace, "AllBattlers", "All Battlers"),
                NumTargets = TargetCount.MultipleTargets,
                TargetsFoe = true,
                TargetsAll = true,
                LongRange = true,
            }
        );

        Register(new Target { Id = "UserSide", Name = Text.Localized(LocalizationNamespace, "UserSide", "User Side") });

        Register(
            new Target
            {
                Id = "FoeSide",
                Name = Text.Localized(LocalizationNamespace, "FoeSide", "Foe Side"),
                AffectsFoeSide = true,
            }
        );

        Register(
            new Target
            {
                Id = "BothSides",
                Name = Text.Localized(LocalizationNamespace, "BothSides", "Both Sides"),
                AffectsFoeSide = true,
            }
        );
    }
    #endregion
}
