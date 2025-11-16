using Injectio.Attributes;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core;
using PokeSharp.Compiler.Mappers;
using PokeSharp.Compiler.Model;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Compiler.Compilers;

[RegisterSingleton<IPbsCompiler>(Duplicate = DuplicateStrategy.Append)]
public class ItemCompiler : PbsCompiler<Item, ItemInfo>
{
    public override int Order => 6;

    protected override Item ConvertToEntity(ItemInfo model) => model.ToGameData();

    protected override ItemInfo ConvertToModel(Item entity) => entity.ToDto();
}
