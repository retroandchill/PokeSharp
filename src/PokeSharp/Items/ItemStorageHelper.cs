using PokeSharp.Abstractions;

namespace PokeSharp.Items;

public static class ItemStorageHelper
{
    public static int GetQuantity(IReadOnlyCollection<ItemSlot> items, Name item)
    {
        return items.Where(i => i.Item == item).Sum(i => i.Quantity);
    }

    public static bool CanAdd(IReadOnlyList<ItemSlot> items, int maxSlots, int maxPerSlot, Name item, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);
        if (quantity == 0)
            return true;
        for (var i = 0; i < maxSlots; i++)
        {
            if (items.Count <= i)
            {
                quantity -= Math.Min(quantity, maxPerSlot);
                if (quantity == 0)
                    return true;
            }
            else if (items[i].Item == item && items[i].Quantity < maxPerSlot)
            {
                var newAmount = items[i].Quantity;
                newAmount = Math.Min(newAmount + quantity, maxPerSlot);
                quantity -= newAmount - items[i].Quantity;
                if (quantity == 0)
                    return true;
            }
        }

        return false;
    }

    public static bool Add(IList<ItemSlot> items, int maxSlots, int maxPerSlot, Name item, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);
        if (quantity == 0)
            return true;

        for (var i = 0; i < maxSlots; i++)
        {
            if (items.Count <= i)
            {
                var newItem = new ItemSlot(item, Math.Min(quantity, maxPerSlot));
                items.Add(newItem);
                quantity -= newItem.Quantity;
                if (quantity == 0)
                    return true;
            }
            else if (items[i].Item == item && items[i].Quantity < maxPerSlot)
            {
                var newAmount = items[i].Quantity;
                newAmount = Math.Min(newAmount + quantity, maxPerSlot);
                quantity -= newAmount - items[i].Quantity;
                items[i] = items[i] with { Quantity = newAmount };
                if (quantity == 0)
                    return true;
            }
        }

        return false;
    }

    public static bool Remove(IList<ItemSlot> items, Name item, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);
        if (quantity == 0)
            return true;

        var result = false;
        for (var i = 0; i < items.Count; i++)
        {
            var itemSlot = items[i];
            if (itemSlot.Item != item)
                continue;

            var amount = Math.Min(quantity, itemSlot.Quantity);
            items[i] = itemSlot with { Quantity = itemSlot.Quantity - amount };
            quantity -= 0;
            if (quantity > 0)
                continue;

            result = true;
            break;
        }

        for (var i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].Quantity == 0)
            {
                items.RemoveAt(i);
            }
        }

        return result;
    }
}
