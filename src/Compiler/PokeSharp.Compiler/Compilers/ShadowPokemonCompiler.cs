using Injectio.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class ShadowPokemonCompiler(
    ILogger<ShadowPokemonCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings
) : PbsCompiler<ShadowPokemon, ShadowPokemonInfo>(logger, pbsCompileSettings)
{
    public override int Order => 10;

    protected override ShadowPokemon ConvertToEntity(ShadowPokemonInfo model) => model.ToGameData();

    protected override ShadowPokemonInfo ConvertToModel(ShadowPokemon entity) => entity.ToDto();
}
