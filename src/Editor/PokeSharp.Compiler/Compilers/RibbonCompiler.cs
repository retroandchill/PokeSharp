using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class RibbonCompiler(
    ILogger<RibbonCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
    PbsSerializer serializer
) : PbsCompiler<Ribbon, RibbonInfo>(logger, pbsCompileSettings, serializer)
{
    public override int Order => 12;

    protected override Ribbon ConvertToEntity(RibbonInfo model) => model.ToGameData();

    protected override RibbonInfo ConvertToModel(Ribbon entity) => entity.ToDto();
}
