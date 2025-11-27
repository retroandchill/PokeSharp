using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Editor.Data;

public class SpeciesOptionSource : INameOptionsSource
{
    public (int Count, IEnumerable<Name> Names) Options
    {
        get
        {
            var speciesKeys = Species.Keys.Where(x => x.Form == 0).Select(x => x.Species).ToArray();

            return (speciesKeys.Length, speciesKeys);
        }
    }
}
