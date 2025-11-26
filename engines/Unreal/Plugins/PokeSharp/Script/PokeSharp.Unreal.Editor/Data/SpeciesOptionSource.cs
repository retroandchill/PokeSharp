using PokeSharp.Data.Pbs;
using PokeSharp.Unreal.Core.Strings;
using UnrealSharp.Core;

namespace PokeSharp.Unreal.Editor.Data;

public class SpeciesOptionSource : INameOptionsSource
{
    public (int Count, IEnumerable<FName> Names) Options
    {
        get
        {
            var speciesKeys = Species.Keys.Where(x => x.Form == 0)
                .Select(x => x.Species.ToUnrealName())
                .ToArray();
            
            return (speciesKeys.Length, speciesKeys);
        }
    }
}