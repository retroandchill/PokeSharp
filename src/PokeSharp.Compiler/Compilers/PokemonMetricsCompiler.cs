using Injectio.Attributes;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class PokemonMetricsCompiler : PbsCompiler<SpeciesMetrics, SpeciesMetricsInfo>
{
    public override int Order => 10;

    protected override SpeciesMetrics ConvertToEntity(SpeciesMetricsInfo model) => model.ToGameData();

    protected override SpeciesMetricsInfo ConvertToModel(SpeciesMetrics entity) => entity.ToDto();
}
