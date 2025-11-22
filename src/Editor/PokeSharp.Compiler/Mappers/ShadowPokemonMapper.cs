using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ShadowPokemonMapper
{
    public static partial ShadowPokemon ToGameData(this ShadowPokemonInfo dto);

    public static partial ShadowPokemonInfo ToDto(this ShadowPokemon entity);
}
