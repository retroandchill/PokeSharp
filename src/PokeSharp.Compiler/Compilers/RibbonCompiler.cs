using Injectio.Attributes;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton<IPbsCompiler>(Duplicate = DuplicateStrategy.Append)]
public class RibbonCompiler : PbsCompiler<Ribbon, RibbonInfo>
{
    public override int Order => 12;

    protected override Ribbon ConvertToEntity(RibbonInfo model) => model.ToGameData();

    protected override RibbonInfo ConvertToModel(Ribbon entity) => entity.ToDto();
}
