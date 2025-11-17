using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.PokemonModel;

public interface IPokemonComponent
{
    Name Id { get; }

    IPokemonComponent Clone(Pokemon newPokemon);
}

public interface IPokemonComponent<out TSelf> : IPokemonComponent
    where TSelf : IPokemonComponent<TSelf>
{
    static abstract Name ComponentId { get; }
}

public interface IPokemonComponentFactory
{
    int Priority { get; }

    bool CanCreate(Pokemon pokemon);

    IPokemonComponent Create(Pokemon pokemon);
}

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
[AutoServiceShortcut]
public sealed class PokemonComponentService(IEnumerable<IPokemonComponentFactory> factories)
{
    private readonly ImmutableArray<IPokemonComponentFactory> _factories = [.. factories.OrderBy(x => x.Priority)];

    public IEnumerable<IPokemonComponent> CreateComponents(Pokemon pokemon)
    {
        return _factories.Where(x => x.CanCreate(pokemon)).Select(x => x.Create(pokemon));
    }
}
