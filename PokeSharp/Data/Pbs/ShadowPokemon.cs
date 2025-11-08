using System.Collections.Immutable;
using PokeSharp.Abstractions;
using PokeSharp.SourceGenerator.Attributes;

namespace PokeSharp.Data.Pbs;

[GameDataEntity( DataPath = "shadow_pokemon")]
public partial record ShadowPokemon
{
    public static ShadowPokemon GetSpeciesForm(Name species, int form) => Get(new SpeciesForm(species, form));
    
    public required SpeciesForm Id { get; init; }
    
    public Name SpeciesId => Id.Species;
    
    public int Form => Id.Form;

    public int GaugeSize { get; init; } = 4000;
    
    public ImmutableArray<Name> Moves { get; init; } = [];
    
    public IReadOnlySet<Name> Flags { get; init; } = ImmutableHashSet<Name>.Empty;
    
    public bool HasFlag(Name flag) => Flags.Contains(flag);
    
}