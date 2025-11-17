using System.Collections.Immutable;
using System.Text.Json.Serialization;
using MessagePack;
using PokeSharp.Abstractions;
using PokeSharp.Core;
using PokeSharp.Data.Core;
using PokeSharp.Data.Pbs;
using PokeSharp.PokemonModel;

namespace PokeSharp.Services.Evolution;

public static class EvolutionExtensions
{
    extension(Pokemon pokemon)
    {
        public bool RemoveHoldItemAfterEvolution(Name evoSpecies, Name parameter, Name newSpecies)
        {
            if (evoSpecies != newSpecies || !pokemon.HasSpecificItem(parameter))
                return false;

            pokemon.Item = null;

            return true;
        }
    }

    extension(Species species)
    {
        public IEnumerable<EvolutionInfo> GetEvolutions(bool excludeInvalid = false)
        {
            return species.Evolutions.Where(evo => !evo.IsPrevious && (!evo.EvolutionMethod.IsNone || !excludeInvalid));
        }

        public IEnumerable<EvolutionFamily> GetFamilyEvolutions(bool excludeInvalid = true)
        {
            return species
                .GetEvolutions(excludeInvalid)
                .OrderBy(e =>
                    Species.Keys.Index().Where(i => i.Item == e.Species).Select(i => i.Index).FirstOrDefault()
                )
                .Select(e => new EvolutionFamily(
                    species.SpeciesId,
                    e.Species,
                    e.EvolutionMethod,
                    e.Parameter,
                    e.IsPrevious
                ))
                .SelectMany(e => new[] { e }.Concat(Species.Get(e.Species).GetFamilyEvolutions(excludeInvalid)));
        }

        public Name PreviousSpecies
        {
            get
            {
                foreach (var evo in species.Evolutions.Where(evo => evo.IsPrevious))
                {
                    return evo.Species;
                }

                return species.SpeciesId;
            }
        }

        public Name GetBabySpecies(bool checkItems = false, Name item1 = default, Name item2 = default)
        {
            if (species.Evolutions.Length == 0)
                return species.SpeciesId;

            var result = species.SpeciesId;
            foreach (var evo in species.Evolutions.Where(evo => evo.IsPrevious))
            {
                if (checkItems)
                {
                    var incense = Species.Get(evo.Species).Incense;
                    if (incense.IsNone || incense == item1 || incense == item2)
                    {
                        result = evo.Species;
                    }
                }
                else
                {
                    result = evo.Species;
                }
            }

            return result != species.SpeciesId ? Species.Get(result).GetBabySpecies(checkItems, item1, item2) : result;
        }

        public IEnumerable<Name> GetFamilySpecies()
        {
            var babySpecies = species.GetBabySpecies();
            yield return babySpecies;

            foreach (var evo in Species.Get(babySpecies).GetFamilyEvolutions(false))
            {
                yield return evo.Species;
            }
        }

        public bool BreedingCanProduce(Name otherSpecies)
        {
            var otherFamily = Species.Get(otherSpecies).GetFamilySpecies();
            return species.Offspring.Length > 0
                ? otherFamily.Intersect(species.Offspring).Any()
                : otherFamily.Contains(species.SpeciesId);
        }

        public ImmutableArray<Name> GetEggMoves()
        {
            if (species.EggMoves.Length > 0)
                return species.EggMoves;

            var previousSpecies = species.PreviousSpecies;
            return previousSpecies != species.SpeciesId
                ? Species.Get(previousSpecies, species.Form).GetEggMoves()
                : species.EggMoves;
        }

        public bool FamilyEvolutionsHaveMethod(Name method, object? parameter = null)
        {
            foreach (var evo in Species.Get(species.GetBabySpecies()).GetFamilyEvolutions())
            {
                if (evo.EvolutionMethod != method)
                    continue;

                if (parameter is not null && parameter.Equals(evo.Parameter))
                    return true;
            }

            return false;
        }

        public bool FamilyItemEvolutionsUseItem(Name item = default)
        {
            var evolutionService = GameServices.EvolutionService;
            return Species
                .Get(species.GetBabySpecies())
                .GetFamilyEvolutions()
                .Where(evo => !evolutionService.MethodHasFlag(evo.EvolutionMethod, EvolutionConditionFlags.UseItem))
                .Any(evo => item.IsValid && item.Equals(evo.Parameter));
        }

        [IgnoreMember]
        [JsonIgnore]
        public int MinimumLevel
        {
            get
            {
                if (species.Evolutions.Length == 0)
                    return 1;

                var evo = species.Evolutions.FirstOrDefault(e => !e.IsPrevious);
                if (evo is null)
                    return 1;

                var previousData = Species.Get(evo.Species, species.BaseForm);
                var previousMinLevel = previousData.MinimumLevel;

                if (previousData.Incense.IsValid)
                    return 1;

                var evolutionMethodData = Data.Core.Evolution.Get(evo.EvolutionMethod);
                if (
                    GameServices.EvolutionService.MethodHasFlag(evo.EvolutionMethod, EvolutionConditionFlags.LevelUp)
                    && evolutionMethodData.Id != Data.Core.Evolution.Shedinja.Id
                )
                    return previousMinLevel;

                return evolutionMethodData.AnyLevelUp ? previousMinLevel + 1 : (int)evo.Parameter!;
            }
        }
    }
}
