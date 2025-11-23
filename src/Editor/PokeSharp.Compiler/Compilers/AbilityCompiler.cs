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
public class AbilityCompiler(
    ILogger<AbilityCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
    PbsSerializer serializer
) : PbsCompiler<Ability, AbilityInfo>(logger, pbsCompileSettings, serializer)
{
    public override int Order => 4;

    protected override Ability ConvertToEntity(AbilityInfo model) => model.ToGameData();

    protected override AbilityInfo ConvertToModel(Ability entity) => entity.ToDto();
}
