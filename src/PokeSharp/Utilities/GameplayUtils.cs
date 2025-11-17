using System.Collections.Immutable;
using PokeSharp.Core;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Utilities;

public static class GameplayUtils
{
    public static int? CurrentRegion => null;

    extension(RegionalDex)
    {
        public static int GetRegionalNumber(int region, Name species)
        {
            if (!RegionalDex.TryGet(region, out var dexList))
                return 0;

            if (!Species.TryGet(species, out var speciesData))
            {
                return 0;
            }

            foreach (var (index, s) in dexList.Entries.Index())
            {
                if (s == speciesData.SpeciesId)
                    return index + 1;
            }

            return 0;
        }

        public static ImmutableArray<Name> GetAllRegionalSpecies(int regionDex)
        {
            if (regionDex < 0)
                return [];

            return RegionalDex.TryGet(regionDex, out var dex) ? dex.Entries : [];
        }
    }
}
