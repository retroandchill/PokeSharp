using PokeSharp.Core;
using PokeSharp.Data.Core;
using PokeSharp.PokemonModel;
using PokeSharp.Trainers;

namespace PokeSharp.Compiler.Sample;

public static class PrintTools
{
    private static readonly Dictionary<Name, string> ShortStats = new()
    {
        [Stat.HP.Id] = "HP",
        [Stat.Attack.Id] = "Atk",
        [Stat.Defense.Id] = "Def",
        [Stat.SpecialAttack.Id] = "SpA",
        [Stat.SpecialDefense.Id] = "SpD",
        [Stat.Speed.Id] = "Spe",
    };

    public static void PrintParty(this Trainer player)
    {
        foreach (var pokemon in player.Party)
        {
            Console.WriteLine(
                pokemon.HasItem ? $"{pokemon.SpeciesData.Name} @ {pokemon.Item.Name}" : pokemon.SpeciesData.Name
            );
            Console.WriteLine($"Level: {pokemon.Level}");
            if (pokemon.Ability is not null)
            {
                Console.WriteLine($"Ability: {pokemon.Ability.Name}");
            }

            var ivs = new List<string>();
            var evs = new List<string>();
            foreach (var stat in Stat.AllMain)
            {
                var iv = pokemon.IV[stat.Id];
                if (iv != Pokemon.IVStatLimit)
                    ivs.Add($"{iv} {ShortStats[stat.Id]}");

                var ev = pokemon.EV[stat.Id];
                if (ev > 0)
                    evs.Add($"{ev} {ShortStats[stat.Id]}");
            }
            if (ivs.Count > 0)
                Console.WriteLine($"IVs: {string.Join(" / ", ivs)}");

            if (evs.Count > 0)
                Console.WriteLine($"EVs: {string.Join(" / ", evs)}");

            if (pokemon.Nature is not null)
            {
                Console.WriteLine($"{pokemon.Nature.Name} Nature");
            }

            var components = pokemon.Components.ToArray();
            if (components.Length > 0)
            {
                Console.WriteLine($"Components: {string.Join(", ", components.Select(x => x.Id))}");
            }

            foreach (var move in pokemon.Moves)
            {
                Console.WriteLine($"- {move.Name}");
            }

            Console.WriteLine();
        }
    }
}
