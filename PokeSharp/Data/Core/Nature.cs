using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

public readonly record struct StatChange(Name Stat, int Change);

[GameDataEntity]
public partial record Nature
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public ImmutableArray<StatChange> StatChanges { get; init; } = [];

    #region Defaults

    private const string LocalizationNamespace = "GameData.Nature";

    public static void AddDefaultValues()
    {
        Register(
            new Nature
            {
                Id = "HARDY",
                Name = Text.Localized(LocalizationNamespace, "HARDY", "Hardy"),
            }
        );

        Register(
            new Nature
            {
                Id = "LONELY",
                Name = Text.Localized(LocalizationNamespace, "LONELY", "Lonely"),
                StatChanges = [new StatChange(Stat.ATTACK, 10), new StatChange(Stat.DEFENSE, -10)],
            }
        );

        Register(
            new Nature
            {
                Id = "BRAVE",
                Name = Text.Localized(LocalizationNamespace, "BRAVE", "Brave"),
                StatChanges = [new StatChange(Stat.ATTACK, 10), new StatChange(Stat.SPEED, -10)],
            }
        );

        Register(
            new Nature
            {
                Id = "ADAMANT",
                Name = Text.Localized(LocalizationNamespace, "ADAMANT", "Adamant"),
                StatChanges =
                [
                    new StatChange(Stat.ATTACK, 10),
                    new StatChange(Stat.SPECIAL_ATTACK, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "NAUGHTY",
                Name = Text.Localized(LocalizationNamespace, "NAUGHTY", "Naughty"),
                StatChanges =
                [
                    new StatChange(Stat.ATTACK, 10),
                    new StatChange(Stat.SPECIAL_DEFENSE, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "BOLD",
                Name = Text.Localized(LocalizationNamespace, "BOLD", "Bold"),
                StatChanges = [new StatChange(Stat.DEFENSE, 10), new StatChange(Stat.ATTACK, -10)],
            }
        );

        Register(
            new Nature
            {
                Id = "DOCILE",
                Name = Text.Localized(LocalizationNamespace, "DOCILE", "Docile"),
            }
        );

        Register(
            new Nature
            {
                Id = "RELAXED",
                Name = Text.Localized(LocalizationNamespace, "RELAXED", "Relaxed"),
                StatChanges = [new StatChange(Stat.DEFENSE, 10), new StatChange(Stat.SPEED, -10)],
            }
        );

        Register(
            new Nature
            {
                Id = "IMPISH",
                Name = Text.Localized(LocalizationNamespace, "IMPISH", "Impish"),
                StatChanges =
                [
                    new StatChange(Stat.DEFENSE, 10),
                    new StatChange(Stat.SPECIAL_ATTACK, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "LAX",
                Name = Text.Localized(LocalizationNamespace, "LAX", "Lax"),
                StatChanges =
                [
                    new StatChange(Stat.DEFENSE, 10),
                    new StatChange(Stat.SPECIAL_DEFENSE, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "TIMID",
                Name = Text.Localized(LocalizationNamespace, "TIMID", "Timid"),
                StatChanges = [new StatChange(Stat.SPEED, 10), new StatChange(Stat.ATTACK, -10)],
            }
        );

        Register(
            new Nature
            {
                Id = "HASTY",
                Name = Text.Localized(LocalizationNamespace, "HASTY", "Hasty"),
                StatChanges = [new StatChange(Stat.SPEED, 10), new StatChange(Stat.DEFENSE, -10)],
            }
        );

        Register(
            new Nature
            {
                Id = "SERIOUS",
                Name = Text.Localized(LocalizationNamespace, "SERIOUS", "Serious"),
            }
        );

        Register(
            new Nature
            {
                Id = "JOLLY",
                Name = Text.Localized(LocalizationNamespace, "JOLLY", "Jolly"),
                StatChanges =
                [
                    new StatChange(Stat.SPEED, 10),
                    new StatChange(Stat.SPECIAL_ATTACK, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "NAIVE",
                Name = Text.Localized(LocalizationNamespace, "NAIVE", "Naive"),
                StatChanges =
                [
                    new StatChange(Stat.SPEED, 10),
                    new StatChange(Stat.SPECIAL_DEFENSE, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "MODEST",
                Name = Text.Localized(LocalizationNamespace, "MODEST", "Modest"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_ATTACK, 10),
                    new StatChange(Stat.ATTACK, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "MILD",
                Name = Text.Localized(LocalizationNamespace, "MILD", "Mild"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_ATTACK, 10),
                    new StatChange(Stat.DEFENSE, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "QUIET",
                Name = Text.Localized(LocalizationNamespace, "QUIET", "Quiet"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_ATTACK, 10),
                    new StatChange(Stat.SPEED, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "BASHFUL",
                Name = Text.Localized(LocalizationNamespace, "BASHFUL", "Bashful"),
            }
        );

        Register(
            new Nature
            {
                Id = "RASH",
                Name = Text.Localized(LocalizationNamespace, "RASH", "Rash"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_ATTACK, 10),
                    new StatChange(Stat.SPECIAL_DEFENSE, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "CALM",
                Name = Text.Localized(LocalizationNamespace, "CALM", "Calm"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_DEFENSE, 10),
                    new StatChange(Stat.ATTACK, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "GENTLE",
                Name = Text.Localized(LocalizationNamespace, "GENTLE", "Gentle"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_DEFENSE, 10),
                    new StatChange(Stat.DEFENSE, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "SASSY",
                Name = Text.Localized(LocalizationNamespace, "SASSY", "Sassy"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_DEFENSE, 10),
                    new StatChange(Stat.SPEED, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "CAREFUL",
                Name = Text.Localized(LocalizationNamespace, "CAREFUL", "Careful"),
                StatChanges =
                [
                    new StatChange(Stat.SPECIAL_DEFENSE, 10),
                    new StatChange(Stat.SPECIAL_ATTACK, -10),
                ],
            }
        );

        Register(
            new Nature
            {
                Id = "QUIRKY",
                Name = Text.Localized(LocalizationNamespace, "QUIRKY", "Quirky"),
            }
        );
    }
    #endregion
}
