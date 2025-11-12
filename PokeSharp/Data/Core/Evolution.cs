using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using PokeSharp.Core.Settings;
using PokeSharp.Core.State;
using PokeSharp.Data.Pbs;
using PokeSharp.Game;
using PokeSharp.Game.Items;
using PokeSharp.Services;
using PokeSharp.Services.DayNightCycle;
using PokeSharp.Services.Overworld;
using PokeSharp.SourceGenerator.Attributes;
using PokeSharp.Utilities;

namespace PokeSharp.Data.Core;

public interface IEvolutionDelegates
{
    Type? Parameter { get; }

    Delegate? LevelUpPredicate { get; }
    Delegate? UseItemPredicate { get; }
    Delegate? EvolutionPredicate { get; }
    Delegate? OnTradePredicate { get; }
    Delegate? AfterBattlePredicate { get; }
    Delegate? EventPredicate { get; }
    Delegate? AfterEvolutionCallback { get; }

    bool CallOnLevelUp(Pokemon pokemon, object? parameter);
    bool CallUseItem(Pokemon pokemon, object? parameter, Name itemUsed);
    bool CallOnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon);
    bool CallAfterBattle(Pokemon pokemon, int partyIndex, object? parameter);
    bool CallEvent(Pokemon pokemon, object? parameter, object? value);
    bool CallAfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies);
}

public class EvolutionDelegates : IEvolutionDelegates
{
    public Type? Parameter => null;

    Delegate? IEvolutionDelegates.LevelUpPredicate => LevelUpPredicate;
    public Func<Pokemon, bool>? LevelUpPredicate { get; init; }

    Delegate? IEvolutionDelegates.UseItemPredicate => UseItemPredicate;
    public Func<Pokemon, Name, bool>? UseItemPredicate { get; init; }

    Delegate? IEvolutionDelegates.EvolutionPredicate => EvolutionPredicate;
    public Func<Pokemon, bool>? EvolutionPredicate { get; init; }

    Delegate? IEvolutionDelegates.OnTradePredicate => OnTradePredicate;
    public Func<Pokemon, Pokemon, bool>? OnTradePredicate { get; init; }

    Delegate? IEvolutionDelegates.AfterBattlePredicate => AfterBattlePredicate;
    public Func<Pokemon, int, bool>? AfterBattlePredicate { get; init; }

    Delegate? IEvolutionDelegates.EventPredicate => EventPredicate;
    public Func<Pokemon, object?, bool>? EventPredicate { get; init; }

    Delegate? IEvolutionDelegates.AfterEvolutionCallback => AfterEvolutionCallback;
    public Func<Pokemon, Name, Name, bool>? AfterEvolutionCallback { get; init; }

    public bool CallOnLevelUp(Pokemon pokemon, object? parameter)
    {
        return LevelUpPredicate is not null && parameter is null && LevelUpPredicate(pokemon);
    }

    public bool CallUseItem(Pokemon pokemon, object? parameter, Name itemUsed)
    {
        return UseItemPredicate is not null && parameter is null && UseItemPredicate(pokemon, itemUsed);
    }

    public bool CallOnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon)
    {
        return OnTradePredicate is not null && parameter is null && OnTradePredicate(pokemon, otherPokemon);
    }

    public bool CallAfterBattle(Pokemon pokemon, int partyIndex, object? parameter)
    {
        return AfterBattlePredicate is not null && parameter is null && AfterBattlePredicate(pokemon, partyIndex);
    }

    public bool CallEvent(Pokemon pokemon, object? parameter, object? value)
    {
        return EventPredicate is not null && parameter is null && EventPredicate(pokemon, value);
    }

    public bool CallAfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies)
    {
        return AfterEvolutionCallback is not null
            && parameter is null
            && AfterEvolutionCallback(pokemon, evoSpecies, newSpecies);
    }
}

public class EvolutionDelegates<T> : IEvolutionDelegates
{
    public Type Parameter => typeof(T);

    Delegate? IEvolutionDelegates.LevelUpPredicate => LevelUpPredicate;
    public Func<Pokemon, T, bool>? LevelUpPredicate { get; init; }

    Delegate? IEvolutionDelegates.UseItemPredicate => UseItemPredicate;
    public Func<Pokemon, T, Name, bool>? UseItemPredicate { get; init; }

    Delegate? IEvolutionDelegates.EvolutionPredicate => EvolutionPredicate;
    public Func<Pokemon, T, bool>? EvolutionPredicate { get; init; }

    Delegate? IEvolutionDelegates.OnTradePredicate => OnTradePredicate;
    public Func<Pokemon, T, Pokemon, bool>? OnTradePredicate { get; init; }

    Delegate? IEvolutionDelegates.AfterBattlePredicate => AfterBattlePredicate;
    public Func<Pokemon, int, T, bool>? AfterBattlePredicate { get; init; }

    Delegate? IEvolutionDelegates.EventPredicate => EventPredicate;
    public Func<Pokemon, T, T, bool>? EventPredicate { get; init; }

    Delegate? IEvolutionDelegates.AfterEvolutionCallback => AfterEvolutionCallback;
    public Func<Pokemon, Name, T, Name, bool>? AfterEvolutionCallback { get; init; }

    public bool CallOnLevelUp(Pokemon pokemon, object? parameter)
    {
        return LevelUpPredicate is not null && parameter is T cast && LevelUpPredicate(pokemon, cast);
    }

    public bool CallUseItem(Pokemon pokemon, object? parameter, Name itemUsed)
    {
        return UseItemPredicate is not null && parameter is T cast && UseItemPredicate(pokemon, cast, itemUsed);
    }

    public bool CallOnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon)
    {
        return OnTradePredicate is not null && parameter is T cast && OnTradePredicate(pokemon, cast, otherPokemon);
    }

    public bool CallAfterBattle(Pokemon pokemon, int partyIndex, object? parameter)
    {
        return AfterBattlePredicate is not null
            && parameter is T cast
            && AfterBattlePredicate(pokemon, partyIndex, cast);
    }

    public bool CallEvent(Pokemon pokemon, object? parameter, object? value)
    {
        return EventPredicate is not null
            && parameter is T cast
            && value is T castValue
            && EventPredicate(pokemon, cast, castValue);
    }

    public bool CallAfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies)
    {
        return AfterEvolutionCallback is not null
            && parameter is T cast
            && AfterEvolutionCallback(pokemon, evoSpecies, cast, newSpecies);
    }
}

public class EvolutionDelegates<TKey, TEntity> : IEvolutionDelegates
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>
{
    public Type Parameter => typeof(TEntity);

    Delegate? IEvolutionDelegates.LevelUpPredicate => LevelUpPredicate;
    public Func<Pokemon, TKey, bool>? LevelUpPredicate { get; init; }

    Delegate? IEvolutionDelegates.UseItemPredicate => UseItemPredicate;
    public Func<Pokemon, TKey, Name, bool>? UseItemPredicate { get; init; }

    Delegate? IEvolutionDelegates.EvolutionPredicate => EvolutionPredicate;
    public Func<Pokemon, TKey, bool>? EvolutionPredicate { get; init; }

    Delegate? IEvolutionDelegates.OnTradePredicate => OnTradePredicate;
    public Func<Pokemon, TKey, Pokemon, bool>? OnTradePredicate { get; init; }

    Delegate? IEvolutionDelegates.AfterBattlePredicate => AfterBattlePredicate;
    public Func<Pokemon, int, TKey, bool>? AfterBattlePredicate { get; init; }

    Delegate? IEvolutionDelegates.EventPredicate => EventPredicate;
    public Func<Pokemon, TKey, TKey, bool>? EventPredicate { get; init; }

    Delegate? IEvolutionDelegates.AfterEvolutionCallback => AfterEvolutionCallback;
    public Func<Pokemon, Name, TKey, Name, bool>? AfterEvolutionCallback { get; init; }

    public bool CallOnLevelUp(Pokemon pokemon, object? parameter)
    {
        return LevelUpPredicate is not null && parameter is TKey cast && LevelUpPredicate(pokemon, cast);
    }

    public bool CallUseItem(Pokemon pokemon, object? parameter, Name itemUsed)
    {
        return UseItemPredicate is not null && parameter is TKey cast && UseItemPredicate(pokemon, cast, itemUsed);
    }

    public bool CallOnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon)
    {
        return OnTradePredicate is not null && parameter is TKey cast && OnTradePredicate(pokemon, cast, otherPokemon);
    }

    public bool CallAfterBattle(Pokemon pokemon, int partyIndex, object? parameter)
    {
        return AfterBattlePredicate is not null
            && parameter is TKey cast
            && AfterBattlePredicate(pokemon, partyIndex, cast);
    }

    public bool CallEvent(Pokemon pokemon, object? parameter, object? value)
    {
        return EventPredicate is not null
            && parameter is TKey cast
            && value is TKey castValue
            && EventPredicate(pokemon, cast, castValue);
    }

    public bool CallAfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies)
    {
        return AfterEvolutionCallback is not null
            && parameter is TKey cast
            && AfterEvolutionCallback(pokemon, evoSpecies, cast, newSpecies);
    }
}

/// <summary>
/// Represents an evolution step or method for a Pokémon entity, defining how it can evolve under specific conditions or actions.
/// </summary>
[GameDataEntity]
public partial record Evolution
{
    /// <inheritdoc />
    public required Name Id { get; init; }

    /// <summary>
    /// Gets the name representation associated with the evolution entity. This property is required and immutable.
    /// </summary>
    public required Text Name { get; init; }

    /// <summary>
    /// Represents a configurable parameter associated with the evolution data.
    /// </summary>
    public Type? Parameter => Delegates?.Parameter;

    /// <summary>
    /// Indicates whether any level-up condition is required for the evolution.
    /// </summary>
    public bool AnyLevelUp { get; init; }

    public required IEvolutionDelegates Delegates { get; init; }

    /// <summary>
    /// Gets the procedure or conditions required for leveling up.
    /// </summary>
    public Delegate? LevelUpProc => Delegates?.LevelUpPredicate;

    /// <summary>
    /// Gets the procedure or conditions required when an item is used.
    /// </summary>
    public Delegate? UseItemProc => Delegates?.UseItemPredicate;

    /// <summary>
    /// Indicates the process to be executed during a trade evolution.
    /// </summary>
    public Delegate? OnTradeProc => Delegates?.OnTradePredicate;

    /// <summary>
    /// Gets the identifier of the procedure triggered after a battle.
    /// </summary>
    public Delegate? AfterBattleProc => Delegates?.AfterBattlePredicate;

    /// <summary>
    /// Represents the event procedure associated with this evolution.
    /// </summary>
    public Delegate? EventProc => Delegates?.EventPredicate;

    /// <summary>
    /// Represents the process or procedure that takes place after the evolution of an entity.
    /// </summary>
    public Delegate? AfterEvolutionProc => Delegates?.AfterEvolutionCallback;

    public bool CallOnLevelUp(Pokemon pokemon, object? parameter) => Delegates.CallOnLevelUp(pokemon, parameter);

    public bool CallUseItem(Pokemon pokemon, object? parameter, Name itemUsed) =>
        Delegates.CallUseItem(pokemon, parameter, itemUsed);

    public bool CallOnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon) =>
        Delegates.CallOnTrade(pokemon, parameter, otherPokemon);

    public bool CallAfterBattle(Pokemon pokemon, int partyIndex, object? parameter) =>
        Delegates.CallAfterBattle(pokemon, partyIndex, parameter);

    public bool CallEvent(Pokemon pokemon, object? parameter, object? value) =>
        Delegates.CallEvent(pokemon, parameter, value);

    public bool CallAfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies) =>
        Delegates.CallAfterEvolution(pokemon, evoSpecies, parameter, newSpecies);

    #region Defaults

    private const string LocalizationNamespace = "GameData.Evolution";

    /// <summary>
    /// Adds default values to the Evolution entity.
    /// This method initializes the entity with predefined values that are assumed
    /// to be commonly associated with the Evolution context in the application.
    /// </summary>
    public static void AddDefaultValues()
    {
        Register(
            new Evolution
            {
                Id = "Level",
                Name = Text.Localized(LocalizationNamespace, "Level", "Level"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelMale",
                Name = Text.Localized(LocalizationNamespace, "LevelMale", "LevelMale"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && pkmn.IsMale,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelFemale",
                Name = Text.Localized(LocalizationNamespace, "LevelFemale", "LevelFemale"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && pkmn.IsFemale,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelDay",
                Name = Text.Localized(LocalizationNamespace, "LevelDay", "LevelDay"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && DayNightService.Instance.IsDay,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelNight",
                Name = Text.Localized(LocalizationNamespace, "LevelNight", "LevelNight"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && DayNightService.Instance.IsNight,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelMorning",
                Name = Text.Localized(LocalizationNamespace, "LevelMorning", "LevelMorning"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && DayNightService.Instance.IsMorning,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelAfternoon",
                Name = Text.Localized(LocalizationNamespace, "LevelAfternoon", "LevelAfternoon"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter && DayNightService.Instance.IsAfternoon,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelEvening",
                Name = Text.Localized(LocalizationNamespace, "LevelEvening", "LevelEvening"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && DayNightService.Instance.IsEvening,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelNoWeather",
                Name = Text.Localized(LocalizationNamespace, "LevelNoWeather", "LevelNoWeather"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter && OverworldWeatherService.Instance.WeatherType.IsNone,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSun",
                Name = Text.Localized(LocalizationNamespace, "LevelSun", "LevelSun"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter
                        && OverworldWeatherService.Instance.Weather?.Category == BattleWeather.Sun,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelRain",
                Name = Text.Localized(LocalizationNamespace, "LevelRain", "LevelRain"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter
                        && OverworldWeatherService.Instance.Weather?.Category == BattleWeather.Rain,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSnow",
                Name = Text.Localized(LocalizationNamespace, "LevelSnow", "LevelSnow"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter
                        && OverworldWeatherService.Instance.Weather?.Category == BattleWeather.Hail,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSandstorm",
                Name = Text.Localized(LocalizationNamespace, "LevelSandstorm", "LevelSandstorm"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter
                        && OverworldWeatherService.Instance.Weather?.Category == BattleWeather.Sandstorm,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelCycling",
                Name = Text.Localized(LocalizationNamespace, "LevelCycling", "LevelCycling"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && PokemonGlobal.Instance.IsCycling,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelSurfing",
                Name = Text.Localized(LocalizationNamespace, "LevelSurfing", "LevelSurfing"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && PokemonGlobal.Instance.IsSurfing,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelDiving",
                Name = Text.Localized(LocalizationNamespace, "LevelDiving", "LevelDiving"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && PokemonGlobal.Instance.IsDiving,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LevelDarkness",
                Name = Text.Localized(LocalizationNamespace, "LevelDarkness", "LevelDarkness"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter && GameMap.Instance.HasMetadataTag(MapMetadataTags.DarkMap),
                },
            }
        );

        Name darkType = "DARK";
        Register(
            new Evolution
            {
                Id = "LevelDarkInParty",
                Name = Text.Localized(LocalizationNamespace, "LevelDarkInParty", "LevelDarkInParty"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter && PlayerTrainer.Instance.HasPokemonOfType(darkType),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "AttackGreater",
                Name = Text.Localized(LocalizationNamespace, "AttackGreater", "AttackGreater"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && pkmn.Attack > pkmn.Defense,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "AtkDefEqual",
                Name = Text.Localized(LocalizationNamespace, "AtkDefEqual", "AtkDefEqual"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && pkmn.Attack == pkmn.Defense,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "DefenseGreater",
                Name = Text.Localized(LocalizationNamespace, "DefenseGreater", "DefenseGreater"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter && pkmn.Attack < pkmn.Defense,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Silcoon",
                Name = Text.Localized(LocalizationNamespace, "Silcoon", "Silcoon"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter && ((pkmn.PersonalityValue >> 16) & 0xFFFF) % 10 < 5,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Cascoon",
                Name = Text.Localized(LocalizationNamespace, "Cascoon", "Cascoon"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) =>
                        pkmn.Level >= parmeter && ((pkmn.PersonalityValue >> 16) & 0xFFFF) % 10 >= 5,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Ninjask",
                Name = Text.Localized(LocalizationNamespace, "Ninjask", "Ninjask"),
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parmeter) => pkmn.Level >= parmeter,
                },
            }
        );

        Name pokeBall = "POKEBALL";
        Register(
            new Evolution
            {
                Id = "Shedinja",
                Name = Text.Localized(LocalizationNamespace, "Shedinja", "Shedinja"),
                Delegates = new EvolutionDelegates<int>
                {
                    AfterEvolutionCallback = (pkmn, newSpecies, _, _) =>
                    {
                        if (PlayerTrainer.Instance.IsPartyFull || !Bag.Instance.HasItem(pokeBall))
                            return false;

                        pkmn.DuplicateForEvolution(newSpecies);
                        Bag.Instance.RemoveItem(pokeBall);

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Happiness",
                Name = Text.Localized(LocalizationNamespace, "Happiness", "Happiness"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates
                {
                    LevelUpPredicate = pkmn =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessMale",
                Name = Text.Localized(LocalizationNamespace, "HappinessMale", "HappinessMale"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates
                {
                    LevelUpPredicate = pkmn =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220) && pkmn.IsMale,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessFemale",
                Name = Text.Localized(LocalizationNamespace, "HappinessFemale", "HappinessFemale"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates
                {
                    LevelUpPredicate = pkmn =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220) && pkmn.IsFemale,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessDay",
                Name = Text.Localized(LocalizationNamespace, "HappinessDay", "HappinessDay"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates
                {
                    LevelUpPredicate = pkmn =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220)
                        && DayNightService.Instance.IsDay,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessNight",
                Name = Text.Localized(LocalizationNamespace, "HappinessNight", "HappinessNight"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates
                {
                    LevelUpPredicate = pkmn =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220)
                        && DayNightService.Instance.IsNight,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessMove",
                Name = Text.Localized(LocalizationNamespace, "HappinessMove", "HappinessMove"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Move>
                {
                    LevelUpPredicate = (pkmn, parameter) =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220)
                        && pkmn.Moves.Any(m => m.Id == parameter),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessMoveType",
                Name = Text.Localized(LocalizationNamespace, "HappinessMoveType", "HappinessMoveType"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, PokemonType>
                {
                    LevelUpPredicate = (pkmn, parameter) =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220)
                        && pkmn.Moves.Any(m => m.Type == parameter),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HappinessHoldItem",
                Name = Text.Localized(LocalizationNamespace, "HappinessHoldItem", "HappinessHoldItem"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    LevelUpPredicate = (pkmn, parameter) =>
                        pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220)
                        && pkmn.HasSpecificItem(parameter),
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "MaxHappiness",
                Name = Text.Localized(LocalizationNamespace, "MaxHappiness", "MaxHappiness"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates
                {
                    LevelUpPredicate = pkmn => pkmn.Happiness >= Pokemon.MaxHappiness,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Beauty",
                Name = Text.Localized(LocalizationNamespace, "Beauty", "Beauty"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (pkmn, parameter) => pkmn.Beauty >= parameter,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItem",
                Name = Text.Localized(LocalizationNamespace, "HoldItem", "HoldItem"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    LevelUpPredicate = (pkmn, parameter) => pkmn.HasSpecificItem(parameter),
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItemMale",
                Name = Text.Localized(LocalizationNamespace, "HoldItemMale", "HoldItemMale"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    LevelUpPredicate = (pkmn, parameter) => pkmn.HasSpecificItem(parameter) && pkmn.IsMale,
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItemFemale",
                Name = Text.Localized(LocalizationNamespace, "HoldItemFemale", "HoldItemFemale"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    LevelUpPredicate = (pkmn, parameter) => pkmn.HasSpecificItem(parameter) && pkmn.IsFemale,
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "DayHoldItem",
                Name = Text.Localized(LocalizationNamespace, "DayHoldItem", "DayHoldItem"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    LevelUpPredicate = (pkmn, parameter) =>
                        pkmn.HasSpecificItem(parameter) && DayNightService.Instance.IsDay,
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "NightHoldItem",
                Name = Text.Localized(LocalizationNamespace, "NightHoldItem", "NightHoldItem"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    LevelUpPredicate = (pkmn, parameter) =>
                        pkmn.HasSpecificItem(parameter) && DayNightService.Instance.IsNight,
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HoldItemHappiness",
                Name = Text.Localized(LocalizationNamespace, "HoldItemHappiness", "HoldItemHappiness"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    LevelUpPredicate = (pkmn, parameter) =>
                        pkmn.HasSpecificItem(parameter)
                        && pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220),
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HasMove",
                Name = Text.Localized(LocalizationNamespace, "HasMove", "HasMove"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, Move>
                {
                    LevelUpPredicate = (pkmn, parameter) => pkmn.Moves.Any(m => m.Id == parameter),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HasMoveType",
                Name = Text.Localized(LocalizationNamespace, "HasMoveType", "HasMoveType"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name, PokemonType>
                {
                    LevelUpPredicate = (pkmn, parameter) => pkmn.Moves.Any(m => m.Type == parameter),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "HasInParty",
                Name = Text.Localized(LocalizationNamespace, "HasInParty", "HasInParty"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<SpeciesForm, Species>
                {
                    LevelUpPredicate = (_, parameter) => PlayerTrainer.Instance.HasSpecies(parameter.Species),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Location",
                Name = Text.Localized(LocalizationNamespace, "Location", "Location"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name>
                {
                    LevelUpPredicate = (_, parameter) => GameMap.Instance.MapId == parameter,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "LocationFlag",
                Name = Text.Localized(LocalizationNamespace, "LocationFlag", "LocationFlag"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<Name>
                {
                    LevelUpPredicate = (_, parameter) => GameMap.Instance.HasMetadataTag(parameter),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Region",
                Name = Text.Localized(LocalizationNamespace, "Region", "Region"),
                AnyLevelUp = true,
                Delegates = new EvolutionDelegates<int>
                {
                    LevelUpPredicate = (_, parameter) => GameMap.Instance.RegionId == parameter,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Item",
                Name = Text.Localized(LocalizationNamespace, "Item", "Item"),
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    UseItemPredicate = (_, parameter, item) => item == parameter,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemMale",
                Name = Text.Localized(LocalizationNamespace, "ItemMale", "ItemMale"),
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    UseItemPredicate = (pkmn, parameter, item) => item == parameter && pkmn.IsMale,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemFemale",
                Name = Text.Localized(LocalizationNamespace, "ItemFemale", "ItemFemale"),
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    UseItemPredicate = (pkmn, parameter, item) => item == parameter && pkmn.IsFemale,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemDay",
                Name = Text.Localized(LocalizationNamespace, "ItemDay", "ItemDay"),
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    UseItemPredicate = (_, parameter, item) => item == parameter && DayNightService.Instance.IsDay,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemNight",
                Name = Text.Localized(LocalizationNamespace, "ItemNight", "ItemNight"),
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    UseItemPredicate = (_, parameter, item) => item == parameter && DayNightService.Instance.IsNight,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "ItemHappiness",
                Name = Text.Localized(LocalizationNamespace, "ItemHappiness", "ItemHappiness"),
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    UseItemPredicate = (pkmn, parameter, item) =>
                        item == parameter
                        && pkmn.Happiness >= (GameSettings.Instance.ApplyHappinessSoftCap ? 160 : 220),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Trade",
                Name = Text.Localized(LocalizationNamespace, "Trade", "Trade"),
                Delegates = new EvolutionDelegates { OnTradePredicate = (_, _) => true },
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeMale",
                Name = Text.Localized(LocalizationNamespace, "TradeMale", "TradeMale"),
                Delegates = new EvolutionDelegates { OnTradePredicate = (pkmn, _) => pkmn.IsMale },
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeFemale",
                Name = Text.Localized(LocalizationNamespace, "TradeFemale", "TradeFemale"),
                Delegates = new EvolutionDelegates { OnTradePredicate = (pkmn, _) => pkmn.IsFemale },
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeDay",
                Name = Text.Localized(LocalizationNamespace, "TradeDay", "TradeDay"),
                Delegates = new EvolutionDelegates { OnTradePredicate = (_, _) => DayNightService.Instance.IsDay },
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeNight",
                Name = Text.Localized(LocalizationNamespace, "TradeNight", "TradeNight"),
                Delegates = new EvolutionDelegates { OnTradePredicate = (_, _) => DayNightService.Instance.IsNight },
            }
        );

        Register(
            new Evolution
            {
                Id = "TradeItem",
                Name = Text.Localized(LocalizationNamespace, "TradeItem", "TradeItem"),
                Delegates = new EvolutionDelegates<Name, Item>
                {
                    OnTradePredicate = (pkmn, parameter, _) => pkmn.HasSpecificItem(parameter),
                    AfterEvolutionCallback = (pkmn, newSpecies, parameter, evoSpecies) =>
                    {
                        if (evoSpecies != newSpecies || !pkmn.HasSpecificItem(parameter))
                            return false;

                        pkmn.Item = null;

                        return true;
                    },
                },
            }
        );

        Name everstone = "EVERSTONE";
        Register(
            new Evolution
            {
                Id = "TradeSpecies",
                Name = Text.Localized(LocalizationNamespace, "TradeSpecies", "TradeSpecies"),
                Delegates = new EvolutionDelegates<SpeciesForm, Species>
                {
                    OnTradePredicate = (pkmn, parameter, otherPkmn) =>
                        pkmn.Species == parameter.Species && !otherPkmn.HasSpecificItem(everstone),
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "BattleDealCriticalHit",
                Name = Text.Localized(LocalizationNamespace, "BattleDealCriticalHit", "BattleDealCriticalHit"),
                Delegates = new EvolutionDelegates<int>
                {
                    AfterBattlePredicate = (_, partyIndex, parameter) =>
                        GameTemp.Instance.PartyCriticalHitsDealt.Count > partyIndex
                        && GameTemp.Instance.PartyCriticalHitsDealt[partyIndex] >= parameter,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "Event",
                Name = Text.Localized(LocalizationNamespace, "Event", "Event"),
                Delegates = new EvolutionDelegates<Name>
                {
                    EventPredicate = (_, parameter, value) => parameter == value,
                },
            }
        );

        Register(
            new Evolution
            {
                Id = "EventAfterDamageTaken",
                Name = Text.Localized(LocalizationNamespace, "EventAfterDamageTaken", "EventAfterDamageTaken"),
                Delegates = new EvolutionDelegates<Name>
                {
                    AfterBattlePredicate = (pkmn, partyIndex, _) =>
                    {
                        if (
                            GameTemp.Instance.PartyCriticalHitsDealt.Count > partyIndex
                            && GameTemp.Instance.PartyCriticalHitsDealt[partyIndex] >= 49
                        )
                        {
                            pkmn.ReadyToEvolve = true;
                        }

                        return false;
                    },
                    EventPredicate = (pkmn, parameter, value) => parameter == value && pkmn.ReadyToEvolve,
                },
            }
        );
    }
    #endregion
}
