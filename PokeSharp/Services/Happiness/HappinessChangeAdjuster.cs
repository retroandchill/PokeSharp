using PokeSharp.Game;

namespace PokeSharp.Services.Happiness;

public interface IHappinessChangeAdjuster
{
    int Priority { get; }

    int AdjustHappinessChange(Pokemon pokemon, HappinessChangeMethod method, int change);
}
