using Injectio.Attributes;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton<IPbsCompiler>(Duplicate = DuplicateStrategy.Append)]
public class AbilityCompiler : PbsCompiler<Ability, AbilityInfo>
{
    public override int Order => 4;

    protected override Ability ConvertToEntity(AbilityInfo model) => model.ToGameData();

    protected override AbilityInfo ConvertToModel(Ability entity) => entity.ToDto();
}
