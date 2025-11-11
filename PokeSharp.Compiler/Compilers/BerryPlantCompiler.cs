using Injectio.Attributes;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton<IPbsCompiler>(Duplicate = DuplicateStrategy.Append)]
public class BerryPlantCompiler : PbsCompiler<BerryPlant, BerryPlantInfo>
{
    public override int Order => 7;

    protected override BerryPlant ConvertToEntity(BerryPlantInfo model) => model.ToGameData();

    protected override BerryPlantInfo ConvertToModel(BerryPlant entity) => entity.ToDto();
}
