using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class SpeciesMetricsMapper
{
    public static partial SpeciesMetrics ToGameData(this SpeciesMetricsInfo dto);

    public static partial SpeciesMetricsInfo ToDto(this SpeciesMetrics entity);
}
