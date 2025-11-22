using PokeSharp.Core;
using PokeSharp.PokemonModel;

namespace PokeSharp.BattleSystem.Challenge;

public interface IPokemonRestriction
{
    bool IsValid(Pokemon pokemon);
}

public interface ITeamRestriction
{
    Text ErrorMessage { get; }

    bool IsValid(IReadOnlyList<Pokemon> team);
}

public record MinimumLevelRestriction(int Level) : IPokemonRestriction
{
    public bool IsValid(Pokemon pokemon)
    {
        return pokemon.Level >= Level;
    }
}

public record MaximumLevelRestriction(int Level) : IPokemonRestriction
{
    public bool IsValid(Pokemon pokemon)
    {
        return pokemon.Level <= Level;
    }
}

public record TotalLevelRestriction(int Level) : ITeamRestriction
{
    private static readonly Text StaticErrorMessage = Text.Localized(
        "TotalLevelRestriction",
        "ErrorMessage",
        "The combined levels exceed {0}."
    );

    public Text ErrorMessage => StaticErrorMessage;

    public bool IsValid(IReadOnlyList<Pokemon> team)
    {
        return team.Sum(p => p.Level) <= Level;
    }
}
