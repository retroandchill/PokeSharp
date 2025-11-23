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
public class PokemonMetricsCompiler(
    ILogger<PokemonMetricsCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
    PbsSerializer serializer
) : PbsCompiler<SpeciesMetrics, SpeciesMetricsInfo>(logger, pbsCompileSettings, serializer)
{
    public override int Order => 10;

    protected override SpeciesMetrics ConvertToEntity(SpeciesMetricsInfo model) => model.ToGameData();

    protected override SpeciesMetricsInfo ConvertToModel(SpeciesMetrics entity) => entity.ToDto();
}
