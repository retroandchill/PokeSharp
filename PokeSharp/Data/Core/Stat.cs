using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Core;

public enum StatType : byte
{
    /// <summary>
    /// Main stat, can't be raised/lowered in battle.
    /// </summary>
    Main,

    /// <summary>
    /// Main stat, can be raised/lowered in battle.
    /// </summary>
    MainBattle,

    /// <summary>
    /// Can be raised/lowered in battle, not part of the Pokémon's stats.
    /// </summary>
    Battle,
}

[GameDataEntity]
public partial record Stat
{
    public required Name Id { get; init; }

    public required Text Name { get; init; }

    public required Text NameBrief { get; init; }

    public StatType StatType { get; init; }

    public int PbsOrder { get; init; }

    public static IEnumerable<Stat> AllMain =>
        Stat.Entities.Where(x => x.StatType is StatType.Main or StatType.MainBattle);

    public static IEnumerable<Stat> AllMainBattle =>
        Stat.Entities.Where(x => x.StatType == StatType.MainBattle);

    public static IEnumerable<Stat> AllBattle =>
        Stat.Entities.Where(x => x.StatType is StatType.Battle or StatType.MainBattle);

    #region Defaults

    private const string LocalizationNamespace = "GameData.Stat";

    public static void AddDefaultValues()
    {
        Register(
            new Stat
            {
                Id = "HP",
                Name = Text.Localized(LocalizationNamespace, "HP", "HP"),
                NameBrief = Text.Localized(LocalizationNamespace, "HP", "HP"),
                StatType = StatType.Main,
                PbsOrder = 0,
            }
        );

        Register(
            new Stat
            {
                Id = "ATTACK",
                Name = Text.Localized(LocalizationNamespace, "Attack", "Attack"),
                NameBrief = Text.Localized(LocalizationNamespace, "Atk", "Atk"),
                StatType = StatType.MainBattle,
                PbsOrder = 1,
            }
        );

        Register(
            new Stat
            {
                Id = "DEFENSE",
                Name = Text.Localized(LocalizationNamespace, "Defense", "Defense"),
                NameBrief = Text.Localized(LocalizationNamespace, "Def", "Def"),
                StatType = StatType.MainBattle,
                PbsOrder = 2,
            }
        );

        Register(
            new Stat
            {
                Id = "SPECIAL_ATTACK",
                Name = Text.Localized(LocalizationNamespace, "SpecialAttack", "SpecialAttack"),
                NameBrief = Text.Localized(LocalizationNamespace, "SpAtk", "SpAtk"),
                StatType = StatType.MainBattle,
                PbsOrder = 4,
            }
        );

        Register(
            new Stat
            {
                Id = "SPECIAL_DEFENSE",
                Name = Text.Localized(LocalizationNamespace, "SpecialDefense", "SpecialDefense"),
                NameBrief = Text.Localized(LocalizationNamespace, "SpDef", "SpDef"),
                StatType = StatType.MainBattle,
                PbsOrder = 5,
            }
        );

        Register(
            new Stat
            {
                Id = "SPEED",
                Name = Text.Localized(LocalizationNamespace, "Speed", "Speed"),
                NameBrief = Text.Localized(LocalizationNamespace, "Spd", "Spd"),
                StatType = StatType.MainBattle,
                PbsOrder = 3,
            }
        );

        Register(
            new Stat
            {
                Id = "ACCURACY",
                Name = Text.Localized(LocalizationNamespace, "accuracy", "accuracy"),
                NameBrief = Text.Localized(LocalizationNamespace, "Acc", "Acc"),
                StatType = StatType.Battle,
            }
        );

        Register(
            new Stat
            {
                Id = "EVASION",
                Name = Text.Localized(LocalizationNamespace, "evasiveness", "evasiveness"),
                NameBrief = Text.Localized(LocalizationNamespace, "Eva", "Eva"),
                StatType = StatType.Battle,
            }
        );
    }
    #endregion
}
