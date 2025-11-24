using System.Collections;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Strings;
using PokeSharp.Data.Pbs;

namespace PokeSharp.Items;

[MessagePackObject(true, AllowPrivate = true)]
public partial class PCItemStorage : IReadOnlyList<ItemSlot>
{
    public const int MaxSize = 999;
    public const int MaxPerSlot = 999;

    private readonly List<ItemSlot> _items = [];

    public PCItemStorage()
    {
        foreach (var item in Metadata.Instance.StartItemStorage.Where(Item.Exists))
        {
            Add(item);
        }
    }

    [SerializationConstructor]
    // ReSharper disable once InconsistentNaming
    private PCItemStorage(List<ItemSlot> _items)
    {
        this._items = _items;
    }

    [IgnoreMember]
    public ItemSlot this[int index] => _items[index];

    [IgnoreMember]
    public int Count => _items.Count;

    public int GetQuantity(Name item)
    {
        return ItemStorageHelper.GetQuantity(_items, item);
    }

    public bool CanAdd(Name item, int quantity = 1)
    {
        return ItemStorageHelper.CanAdd(_items, MaxSize, MaxPerSlot, item, quantity);
    }

    public bool Add(Name item, int quantity = 1)
    {
        return ItemStorageHelper.Add(_items, MaxSize, MaxPerSlot, item, quantity);
    }

    public bool Remove(Name item, int quantity = 1)
    {
        return ItemStorageHelper.Remove(_items, item, quantity);
    }

    public IEnumerator<ItemSlot> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items).GetEnumerator();
}
