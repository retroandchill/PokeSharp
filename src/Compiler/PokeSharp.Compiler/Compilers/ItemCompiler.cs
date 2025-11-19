using Injectio.Attributes;
using Microsoft.Extensions.Options;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton(Duplicate = DuplicateStrategy.Append)]
public class ItemCompiler(IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings) : PbsCompiler<Item, ItemInfo>(pbsCompileSettings)
{
    public override int Order => 6;

    protected override Item ConvertToEntity(ItemInfo model) => model.ToGameData();

    protected override ItemInfo ConvertToModel(Item entity) => entity.ToDto();
}
