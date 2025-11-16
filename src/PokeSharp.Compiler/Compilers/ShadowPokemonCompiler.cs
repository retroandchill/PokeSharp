using Injectio.Attributes;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton<IPbsCompiler>(Duplicate = DuplicateStrategy.Append)]
public class ShadowPokemonCompiler : PbsCompiler<ShadowPokemon, ShadowPokemonInfo>
{
    public override int Order => 10;

    protected override ShadowPokemon ConvertToEntity(ShadowPokemonInfo model) => model.ToGameData();

    protected override ShadowPokemonInfo ConvertToModel(ShadowPokemon entity) => entity.ToDto();
}
