using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Core.Data;
using PokeSharp.PokemonModel;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Services.Evolution;

[RegisterSingleton]
[AutoServiceShortcut]
public class EvolutionService
{
    private readonly ImmutableArray<ICanEvolveEvaluator> _canEvolveEvaluators;
    private readonly Dictionary<Name, IEvolutionMethodEvaluator> _evolutionMethodEvaluators;

    public EvolutionService(
        RegisteredGameDataSet<Data.Core.Evolution, Name> evolutionRepo,
        IEnumerable<ICanEvolveEvaluator> canEvolveEvaluators,
        IEnumerable<IEvolutionMethodEvaluator> evolutionMethodEvaluators
    )
    {
        _canEvolveEvaluators = [.. canEvolveEvaluators.OrderBy(e => e.Priority)];
        _evolutionMethodEvaluators = evolutionMethodEvaluators.ToDictionary(e => e.EvolutionMethod);

        foreach (var (method, evaluator) in _evolutionMethodEvaluators)
        {
            var existingMethod = evolutionRepo.Get(method);
            var evaluatorType = evaluator.ParameterType;
            var methodType = existingMethod.Parameter;

            if (
                (evaluatorType is null && methodType is null)
                || (evaluatorType is not null && methodType is not null && evaluatorType.IsAssignableFrom(methodType))
            )
            {
                continue;
            }

            throw new InvalidOperationException(
                $"The evaluator for evolution method {method} is not compatible with the registered parameter type {evaluatorType} for the evolution method."
            );
        }
    }

    public bool CanEvolve(Pokemon pokemon)
    {
        return _canEvolveEvaluators.All(e => e.CanEvolve(pokemon));
    }

    public bool MethodHasFlag(Name method, EvolutionConditionFlags flags)
    {
        return _evolutionMethodEvaluators.TryGetValue(method, out var evaluator)
            && evaluator.EvolutionConditions.HasFlag(flags);
    }

    public bool OnLevelUp(Name method, Pokemon pokemon, object? parameter)
    {
        return _evolutionMethodEvaluators.TryGetValue(method, out var evaluator)
            && evaluator.OnLevelUp(pokemon, parameter);
    }

    public bool OnUseItem(Name method, Pokemon pokemon, object? parameter, Name itemUsed)
    {
        return _evolutionMethodEvaluators.TryGetValue(method, out var evaluator)
            && evaluator.OnUseItem(pokemon, parameter, itemUsed);
    }

    public bool OnTrade(Name method, Pokemon pokemon, object? parameter, Pokemon otherPokemon)
    {
        return _evolutionMethodEvaluators.TryGetValue(method, out var evaluator)
            && evaluator.OnTrade(pokemon, parameter, otherPokemon);
    }

    public bool AfterBattle(Name method, Pokemon pokemon, int partyIndex, object? parameter)
    {
        return _evolutionMethodEvaluators.TryGetValue(method, out var evaluator)
            && evaluator.AfterBattle(pokemon, partyIndex, parameter);
    }

    public bool OnEvent(Name method, Pokemon pokemon, object? parameter, object? value)
    {
        return _evolutionMethodEvaluators.TryGetValue(method, out var evaluator)
            && evaluator.OnEvent(pokemon, parameter, value);
    }

    public bool AfterEvolution(Name method, Pokemon pokemon, Name evoSpecies, object? parameter, Name newSpecies)
    {
        return _evolutionMethodEvaluators.TryGetValue(method, out var evaluator)
            && evaluator.AfterEvolution(pokemon, evoSpecies, parameter, newSpecies);
    }
}
