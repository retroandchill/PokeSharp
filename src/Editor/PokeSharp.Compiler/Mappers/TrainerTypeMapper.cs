using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TrainerTypeMapper
{
    [MapPropertyFromSource(nameof(TrainerTypeInfo.SkillLevel), Use = nameof(MapSkillLevelTo))]
    public static partial TrainerType ToGameData(this TrainerTypeInfo dto);

    [MapPropertyFromSource(nameof(TrainerType.SkillLevel), Use = nameof(MapSkillLevelFrom))]
    public static partial TrainerTypeInfo ToDto(this TrainerType entity);

    private static int MapSkillLevelTo(TrainerTypeInfo trainerTypeInfo)
    {
        return trainerTypeInfo.SkillLevel ?? trainerTypeInfo.BaseMoney;
    }

    private static int? MapSkillLevelFrom(TrainerType trainerType)
    {
        return trainerType.SkillLevel != trainerType.BaseMoney ? trainerType.SkillLevel : null;
    }
}
