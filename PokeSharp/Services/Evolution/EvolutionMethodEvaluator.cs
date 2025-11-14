using System.Reflection;
using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using PokeSharp.Game;

namespace PokeSharp.Services.Evolution;

[Flags]
public enum EvolutionConditionFlags : byte
{
    None = 0,
    LevelUp = 1,
    UseItem = 2,
    Trade = 4,
    AfterBattle = 8,
    Event = 16,
    AfterEvolution = 32,
}

public interface IEvolutionMethodEvaluator
{
    public Name EvolutionMethod { get; }

    public Type? ParameterType { get; }

    public EvolutionConditionFlags EvolutionConditions { get; }

    bool OnLevelUp(Pokemon pokemon, object? parameter);

    bool OnUseItem(Pokemon pokemon, object? parameter, Name itemUsed);

    public bool OnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon);

    bool AfterBattle(Pokemon pokemon, int partyIndex, object? parameter);

    bool OnEvent(Pokemon pokemon, object? parameter, object? value);

    bool AfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies);
}

public abstract class EvolutionMethodEvaluator : IEvolutionMethodEvaluator
{
    public abstract Name EvolutionMethod { get; }
    public Type? ParameterType => null;

    public EvolutionConditionFlags EvolutionConditions { get; }

    internal static readonly Dictionary<string, EvolutionConditionFlags> ConditionFlags = new()
    {
        [nameof(OnLevelUp)] = EvolutionConditionFlags.LevelUp,
        [nameof(OnUseItem)] = EvolutionConditionFlags.UseItem,
        [nameof(OnTrade)] = EvolutionConditionFlags.Trade,
        [nameof(AfterBattle)] = EvolutionConditionFlags.AfterBattle,
        [nameof(OnEvent)] = EvolutionConditionFlags.Event,
        [nameof(AfterEvolution)] = EvolutionConditionFlags.AfterEvolution,
    };

    protected EvolutionMethodEvaluator()
    {
        EvolutionConditions = typeof(EvolutionMethodEvaluator)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.IsVirtual)
            .Select(m => ConditionFlags.GetValueOrDefault(m.Name))
            .Aggregate(EvolutionConditions, (current, flag) => current | flag);
    }

    public bool OnLevelUp(Pokemon pokemon, object? parameter)
    {
        return parameter is null && OnLevelUp(pokemon);
    }

    protected virtual bool OnLevelUp(Pokemon pokemon) => false;

    public bool OnUseItem(Pokemon pokemon, object? parameter, Name itemUsed)
    {
        return parameter is null && OnUseItem(pokemon, itemUsed);
    }

    protected virtual bool OnUseItem(Pokemon pokemon, Name itemUsed) => false;

    public bool OnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon)
    {
        return parameter is null && OnTrade(pokemon, otherPokemon);
    }

    protected virtual bool OnTrade(Pokemon pokemon, Pokemon otherPokemon) => false;

    public bool AfterBattle(Pokemon pokemon, int partyIndex, object? parameter)
    {
        return parameter is null && AfterBattle(pokemon, partyIndex);
    }

    protected virtual bool AfterBattle(Pokemon pokemon, int partyIndex) => false;

    public bool OnEvent(Pokemon pokemon, object? parameter, object? value)
    {
        return parameter is null && OnEvent(pokemon, value);
    }

    protected virtual bool OnEvent(Pokemon pokemon, object? value) => false;

    public bool AfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies)
    {
        return parameter is null && AfterEvolution(pokemon, evoSpecies, newSpecies);
    }

    protected virtual bool AfterEvolution(Pokemon pokemon, Name evoSpecies, Name newSpecies) => false;
}

public abstract class EvolutionMethodEvaluator<T> : IEvolutionMethodEvaluator
    where T : notnull
{
    public abstract Name EvolutionMethod { get; }
    public Type ParameterType => typeof(T);
    public EvolutionConditionFlags EvolutionConditions { get; }

    protected EvolutionMethodEvaluator()
    {
        EvolutionConditions = typeof(EvolutionMethodEvaluator)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.IsVirtual)
            .Select(m => EvolutionMethodEvaluator.ConditionFlags.GetValueOrDefault(m.Name))
            .Aggregate(EvolutionConditions, (current, flag) => current | flag);
    }

    public bool OnLevelUp(Pokemon pokemon, object? parameter)
    {
        return parameter is T asT && OnLevelUp(pokemon, asT);
    }

    protected virtual bool OnLevelUp(Pokemon pokemon, T parameter) => false;

    public bool OnUseItem(Pokemon pokemon, object? parameter, Name itemUsed)
    {
        return parameter is T asT && OnUseItem(pokemon, asT, itemUsed);
    }

    protected virtual bool OnUseItem(Pokemon pokemon, T parameter, Name itemUsed) => false;

    public bool OnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon)
    {
        return parameter is T asT && OnTrade(pokemon, asT, otherPokemon);
    }

    protected virtual bool OnTrade(Pokemon pokemon, T parameter, Pokemon otherPokemon) => false;

    public bool AfterBattle(Pokemon pokemon, int partyIndex, object? parameter)
    {
        return parameter is T asT && AfterBattle(pokemon, partyIndex, asT);
    }

    protected virtual bool AfterBattle(Pokemon pokemon, int partyIndex, T parameter) => false;

    public bool OnEvent(Pokemon pokemon, object? parameter, object? value)
    {
        return parameter is T asT && value is T valueT && OnEvent(pokemon, asT, valueT);
    }

    protected virtual bool OnEvent(Pokemon pokemon, T parameter, T value) => false;

    public bool AfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies)
    {
        return parameter is T asT && AfterEvolution(pokemon, evoSpecies, asT, newSpecies);
    }

    protected virtual bool AfterEvolution(Pokemon pokemon, Name evoSpecies, T parameter, Name newSpecies) => false;
}

public abstract class EvolutionMethodEvaluator<TKey, TEntity> : IEvolutionMethodEvaluator
    where TKey : notnull
    where TEntity : IGameDataEntity<TKey, TEntity>
{
    public abstract Name EvolutionMethod { get; }
    public Type ParameterType => typeof(TEntity);
    public EvolutionConditionFlags EvolutionConditions { get; }

    protected EvolutionMethodEvaluator()
    {
        EvolutionConditions = typeof(EvolutionMethodEvaluator)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(m => m.IsVirtual)
            .Select(m => EvolutionMethodEvaluator.ConditionFlags.GetValueOrDefault(m.Name))
            .Aggregate(EvolutionConditions, (current, flag) => current | flag);
    }

    public bool OnLevelUp(Pokemon pokemon, object? parameter)
    {
        return parameter is TKey asT && OnLevelUp(pokemon, asT);
    }

    protected virtual bool OnLevelUp(Pokemon pokemon, TKey parameter) => false;

    public bool OnUseItem(Pokemon pokemon, object? parameter, Name itemUsed)
    {
        return parameter is TKey asT && OnUseItem(pokemon, asT, itemUsed);
    }

    protected virtual bool OnUseItem(Pokemon pokemon, TKey parameter, Name itemUsed) => false;

    public bool OnTrade(Pokemon pokemon, object? parameter, Pokemon otherPokemon)
    {
        return parameter is TKey asT && OnTrade(pokemon, asT, otherPokemon);
    }

    protected virtual bool OnTrade(Pokemon pokemon, TKey parameter, Pokemon otherPokemon) => false;

    public bool AfterBattle(Pokemon pokemon, int partyIndex, object? parameter)
    {
        return parameter is TKey asT && AfterBattle(pokemon, partyIndex, asT);
    }

    protected virtual bool AfterBattle(Pokemon pokemon, int partyIndex, TKey parameter) => false;

    public bool OnEvent(Pokemon pokemon, object? parameter, object? value)
    {
        return parameter is TKey asT && value is TKey valueT && OnEvent(pokemon, asT, valueT);
    }

    protected virtual bool OnEvent(Pokemon pokemon, TKey parameter, TKey value) => false;

    public bool AfterEvolution(Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies)
    {
        return parameter is TKey asT && AfterEvolution(pokemon, evoSpecies, asT, newSpecies);
    }

    protected virtual bool AfterEvolution(Pokemon pokemon, Name evoSpecies, TKey parameter, Name newSpecies) => false;
}
