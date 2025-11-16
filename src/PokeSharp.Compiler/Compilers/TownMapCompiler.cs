using Injectio.Attributes;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class TownMapCompiler : PbsCompiler<TownMap, TownMapInfo>
{
    public override int Order => 1;

    protected override TownMap ConvertToEntity(TownMapInfo model) => model.ToGameData();

    protected override TownMapInfo ConvertToModel(TownMap entity) => entity.ToDto();
}
