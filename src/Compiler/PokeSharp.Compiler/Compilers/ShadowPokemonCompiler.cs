using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class ShadowPokemonCompiler(IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings) : PbsCompiler<ShadowPokemon, ShadowPokemonInfo>(pbsCompileSettings)
{
    public override int Order => 10;

    protected override ShadowPokemon ConvertToEntity(ShadowPokemonInfo model) => model.ToGameData();

    protected override ShadowPokemonInfo ConvertToModel(ShadowPokemon entity) => entity.ToDto();
}
