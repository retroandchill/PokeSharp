using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class BerryPlantMapper
{
    public static partial BerryPlant ToGameData(this BerryPlantInfo dto);

    public static partial BerryPlantInfo ToDto(this BerryPlant entity);
}
