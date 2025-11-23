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
public class ShadowPokemonCompiler(
    ILogger<ShadowPokemonCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
    PbsSerializer serializer
) : PbsCompiler<ShadowPokemon, ShadowPokemonInfo>(logger, pbsCompileSettings, serializer)
{
    public override int Order => 10;

    protected override ShadowPokemon ConvertToEntity(ShadowPokemonInfo model) => model.ToGameData();

    protected override ShadowPokemonInfo ConvertToModel(ShadowPokemon entity) => entity.ToDto();
}
