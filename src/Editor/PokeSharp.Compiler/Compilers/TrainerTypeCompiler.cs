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
public class TrainerTypeCompiler(
    ILogger<TrainerTypeCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
    PbsSerializer serializer
) : PbsCompiler<TrainerType, TrainerTypeInfo>(logger, pbsCompileSettings, serializer)
{
    public override int Order => 14;

    protected override TrainerType ConvertToEntity(TrainerTypeInfo model) => model.ToGameData();

    protected override TrainerTypeInfo ConvertToModel(TrainerType entity) => entity.ToDto();
}
