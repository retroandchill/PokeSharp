using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class MetadataMapper
{
    public static partial Metadata ToGameData(this MetadataInfo dto);

    public static partial PlayerMetadata ToGameData(this PlayerMetadataInfo dto);

    public static partial MetadataInfo ToDto(this Metadata entity);

    public static partial PlayerMetadataInfo ToDto(this PlayerMetadata entity);
}
