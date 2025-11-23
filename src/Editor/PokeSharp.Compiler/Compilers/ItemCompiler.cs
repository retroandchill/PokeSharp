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
public class ItemCompiler(
    ILogger<ItemCompiler> logger,
    IOptionsMonitor<PbsCompilerSettings> pbsCompileSettings,
    PbsSerializer serializer
) : PbsCompiler<Item, ItemInfo>(logger, pbsCompileSettings, serializer)
{
    public override int Order => 6;

    protected override Item ConvertToEntity(ItemInfo model) => model.ToGameData();

    protected override ItemInfo ConvertToModel(Item entity) => entity.ToDto();
}
