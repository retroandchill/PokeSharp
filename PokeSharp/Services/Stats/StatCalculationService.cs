namespace PokeSharp.Services.Stats;

public readonly record struct StatCalculationResult(
    int MaxHP,
    int Attack,
    int Defense,
    int SpecialAttack,
    int SpecialDefense,
    int Speed
);

public interface StatCalculationService { }
