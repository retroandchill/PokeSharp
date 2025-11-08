using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TypeMapper
{
    public static partial PokemonType ToGameData(this PokemonTypeInfo dto);
    
    public static partial PokemonTypeInfo ToDto(this PokemonType dto);
}