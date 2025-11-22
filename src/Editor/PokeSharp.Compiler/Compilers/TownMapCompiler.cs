using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class TownMapCompiler(ILogger<TownMapCompiler> logger, IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings)
    : PbsCompiler<TownMap, TownMapInfo>(logger, pbsCompileSettings)
{
    public override int Order => 1;

    protected override TownMap ConvertToEntity(TownMapInfo model) => model.ToGameData();

    protected override TownMapInfo ConvertToModel(TownMap entity) => entity.ToDto();
}
