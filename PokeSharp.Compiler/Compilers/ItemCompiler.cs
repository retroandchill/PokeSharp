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

    protected override object? GetPropertyForPbs(ItemInfo model, string key)
    {
        var originalValue = base.GetPropertyForPbs(model, key);
        switch (key)
        {
            case nameof(ItemInfo.BPPrice) when originalValue is 1:
            case nameof(ItemInfo.FieldUse) when originalValue is FieldUse.NoFieldUse:
            case nameof(ItemInfo.BattleUse) when originalValue is BattleUse.NoBattleUse:
            case nameof(ItemInfo.Move) when originalValue is Name { IsNone: true }:
                return null;
            case nameof(ItemInfo.Consumable):
                return model.Consumable;
            default:
                return originalValue;
        }
    }
}
