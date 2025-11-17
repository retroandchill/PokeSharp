using PokeSharp.Compiler.Model;
using PokeSharp.Core;
using PokeSharp.Data.Pbs;
using Riok.Mapperly.Abstractions;

namespace PokeSharp.Compiler.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ItemMapper
{
    [MapPropertyFromSource(nameof(ItemInfo.PortionName), Use = nameof(MapPortionNameTo))]
    [MapPropertyFromSource(nameof(ItemInfo.PortionNamePlural), Use = nameof(MapPortionNamePluralTo))]
    [MapPropertyFromSource(nameof(ItemInfo.SellPrice), Use = nameof(MapSellPriceTo))]
    [MapPropertyFromSource(nameof(ItemInfo.Consumable), Use = nameof(MapConsumableTo))]
    [MapPropertyFromSource(nameof(ItemInfo.ShowQuantity), Use = nameof(MapShowQuantityTo))]
    public static partial Item ToGameData(this ItemInfo dto);

    [MapPropertyFromSource(nameof(Item.PortionName), Use = nameof(MapPortionNameFrom))]
    [MapPropertyFromSource(nameof(Item.PortionNamePlural), Use = nameof(MapPortionNamePluralFrom))]
    [MapPropertyFromSource(nameof(Item.SellPrice), Use = nameof(MapSellPriceFrom))]
    [MapPropertyFromSource(nameof(Item.Consumable), Use = nameof(MapConsumableFrom))]
    [MapPropertyFromSource(nameof(Item.ShowQuantity), Use = nameof(MapShowQuantityFrom))]
    public static partial ItemInfo ToDto(this Item entity);

    private static Text MapPortionNameTo(ItemInfo itemInfo)
    {
        return itemInfo.PortionName ?? itemInfo.Name;
    }

    private static Text MapPortionNamePluralTo(ItemInfo itemInfo)
    {
        return itemInfo.PortionNamePlural ?? itemInfo.NamePlural;
    }

    private static int MapSellPriceTo(ItemInfo itemInfo)
    {
        return itemInfo.SellPrice ?? itemInfo.Price / 2;
    }

    private static bool MapConsumableTo(ItemInfo itemInfo)
    {
        return itemInfo.Consumable ?? true;
    }

    private static bool MapShowQuantityTo(ItemInfo itemInfo)
    {
        return itemInfo.ShowQuantity ?? true;
    }

    private static Text? MapPortionNameFrom(Item item)
    {
        return item.PortionName != item.Name ? item.PortionName : (Text?)null;
    }

    private static Text? MapPortionNamePluralFrom(Item item)
    {
        return item.PortionNamePlural != item.NamePlural ? item.PortionNamePlural : (Text?)null;
    }

    private static int? MapSellPriceFrom(Item item)
    {
        return item.SellPrice != item.Price / 2 ? item.SellPrice : null;
    }

    private static bool? MapConsumableFrom(Item item)
    {
        return item.Consumable || item.IsImportant ? null : item.Consumable;
    }

    private static bool? MapShowQuantityFrom(Item item)
    {
        return item.ShowQuantity || !item.IsImportant ? null : item.ShowQuantity;
    }
}
