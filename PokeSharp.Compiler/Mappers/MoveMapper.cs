using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class MoveMapper
{
    public static partial Move ToGameData(this MoveInfo dto);
    
    public static partial MoveInfo ToDto(this Move entity);
}