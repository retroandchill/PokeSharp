using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class AbilityMapper
{
    public static partial Ability ToGameData(this AbilityInfo dto);

    public static partial AbilityInfo ToDto(this Ability entity);
}
