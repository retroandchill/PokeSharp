using System.Collections.Immutable;
using Injectio.Attributes;
using PokeSharp.Core;
using PokeSharp.PokemonModel;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Services.Happiness;

public record HappinessChangeMethod(Name Id, int Change1, int Change2, int Change3)
{
    public static readonly HappinessChangeMethod Walking = new("Walking", 2, 2, 1);
    public static readonly HappinessChangeMethod LevelUp = new("LevelUp", 5, 4, 3);
    public static readonly HappinessChangeMethod Groom = new("Groom", 10, 10, 4);
    public static readonly HappinessChangeMethod EVBerry = new("EVBerry", 10, 5, 2);
    public static readonly HappinessChangeMethod Vitamin = new("Vitamin", 5, 3, 2);
    public static readonly HappinessChangeMethod Wing = new("Wing", 3, 2, 1);
    public static readonly HappinessChangeMethod Machine = new("Machine", 1, 1, 0);
    public static readonly HappinessChangeMethod BattleItem = new("BattleItem", 1, 1, 0);
    public static readonly HappinessChangeMethod Faint = new("Faint", -1, -1, -1);
    public static readonly HappinessChangeMethod FaintBad = new("FaintBad", -5, -5, -10);
    public static readonly HappinessChangeMethod Powder = new("Powder", -5, -5, -10);
    public static readonly HappinessChangeMethod EnergyRoot = new("EnergyRoot", -10, -10, -15);
    public static readonly HappinessChangeMethod RevivalHerb = new("RevivalHerb", -15, -15, -20);
}

[RegisterSingleton]
[AutoServiceShortcut]
public class HappinessChangeService(
    IEnumerable<IHappinessChangeAdjuster> changeAdjusters,
    IEnumerable<IHappinessChangeBlocker> changeBlockers
)
{
    private static readonly CachedService<HappinessChangeService> InstanceSingleton = new();

    public static HappinessChangeService Instance => InstanceSingleton.Instance;

    private readonly ImmutableArray<IHappinessChangeAdjuster> _changeAdjusters =
    [
        .. changeAdjusters.OrderBy(a => a.Priority),
    ];

    private readonly ImmutableArray<IHappinessChangeBlocker> _changeBlockers =
    [
        .. changeBlockers.OrderBy(b => b.Priority),
    ];

    public void ApplyHappinessChange(Pokemon pokemon, HappinessChangeMethod method)
    {
        if (_changeBlockers.Any(b => b.ShouldBlockHappinessChange(pokemon)))
            return;

        var happinessRange = pokemon.Happiness / 100;
        var change = happinessRange switch
        {
            0 => method.Change1,
            1 => method.Change2,
            2 => method.Change3,
            _ => throw new InvalidOperationException("Happiness range must be between 0 and 2"),
        };

        change = Enumerable.Aggregate(
            _changeAdjusters,
            change,
            (current, adjuster) => adjuster.AdjustHappinessChange(pokemon, method, current)
        );

        pokemon.Happiness = Math.Clamp(pokemon.Happiness + change, 0, Pokemon.MaxHappiness);
    }
}
