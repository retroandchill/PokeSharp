using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class TrainerTypeCompiler(IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings) : PbsCompiler<TrainerType, TrainerTypeInfo>(pbsCompileSettings)
{
    public override int Order => 14;

    protected override TrainerType ConvertToEntity(TrainerTypeInfo model) => model.ToGameData();

    protected override TrainerTypeInfo ConvertToModel(TrainerType entity) => entity.ToDto();
}
