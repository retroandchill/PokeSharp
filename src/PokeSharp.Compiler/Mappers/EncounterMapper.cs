using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class EncounterMapper
{
    public static partial Encounter ToGameData(this EncounterInfo dto);

    public static partial EncounterInfo ToDto(this Encounter entity);
}
