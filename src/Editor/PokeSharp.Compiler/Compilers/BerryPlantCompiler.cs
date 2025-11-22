using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class BerryPlantCompiler(
    ILogger<BerryPlantCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings
) : PbsCompiler<BerryPlant, BerryPlantInfo>(logger, pbsCompileSettings)
{
    public override int Order => 7;

    protected override BerryPlant ConvertToEntity(BerryPlantInfo model) => model.ToGameData();

    protected override BerryPlantInfo ConvertToModel(BerryPlant entity) => entity.ToDto();
}
