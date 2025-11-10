using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class RibbonMapper
{
    public static partial Ribbon ToGameData(this RibbonInfo dto);

    public static partial RibbonInfo ToDto(this Ribbon entity);
}
