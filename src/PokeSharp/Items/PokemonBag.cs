using System.Collections;
using System.Collections.Immutable;
using MessagePack;
using PokeSharp.Core;
using PokeSharp.Core.Utils;
using PokeSharp.Data.Pbs;
using PokeSharp.Settings;

namespace PokeSharp.Items;

[MessagePackObject(true)]
public readonly record struct ItemSlot(Name Item, int Quantity);

public readonly struct ReadOnlyPockets(ImmutableArray<List<ItemSlot>> pockets) : IReadOnlyList<IReadOnlyList<ItemSlot>>
{
    int IReadOnlyCollection<IReadOnlyList<ItemSlot>>.Count => pockets.Length;

    public int Length => pockets.Length;

    public IReadOnlyList<ItemSlot> this[int index] => pockets[index];

    public IEnumerator<IReadOnlyList<ItemSlot>> GetEnumerator() =>
        pockets.Cast<IReadOnlyList<ItemSlot>>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[MessagePackObject(true, AllowPrivate = true)]
public partial class PokemonBag
{
    public static PokemonBag Instance { get; } = new();

    public static IEnumerable<Text> PocketNames => GameGlobal.GameSettings.BagPockets.Select(x => x.Name);

    [IgnoreMember]
    public static int PocketCount => GameGlobal.GameSettings.BagPockets.Length;

    private readonly ImmutableArray<List<ItemSlot>> _pockets;

    [IgnoreMember]
    public ReadOnlyPockets Pockets => new(_pockets);

    public int LastViewedPocket { get; set; } = 1;

    public int[] LastPocketSelections { get; } = new int[PocketCount + 1];

    public List<Name> RegisteredItems { get; } = [];

    public (int, int, int) ReadMenuSelections { get; private set; } = (0, 0, 1);

    public PokemonBag()
    {
        _pockets = [.. Enumerable.Range(0, PocketCount + 1).Select(_ => new List<ItemSlot>())];
        for (var i = 0; i <= PocketCount; i++)
        {
            LastPocketSelections[i] = 0;
        }
    }

    [SerializationConstructor]
    // ReSharper disable once InconsistentNaming
    private PokemonBag(ImmutableArray<List<ItemSlot>> _pockets, int[] lastPocketSelections, List<Name> registeredItems)
    {
        this._pockets = _pockets;
        LastPocketSelections = lastPocketSelections;
        RegisteredItems = registeredItems;
    }

    public void ResetLastSelections()
    {
        LastViewedPocket = 1;
        for (var i = 0; i <= PocketCount; i++)
        {
            LastPocketSelections[i] = 0;
        }
    }

    public void Clear()
    {
        foreach (var pocket in _pockets)
        {
            pocket.Clear();
        }
        for (var i = 1; i <= PocketCount; i++)
        {
            LastPocketSelections[i] = 0;
        }
    }

    public bool GetLastViewedIndex(int pocket)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pocket, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pocket, PocketCount);
        return Math.Clamp(LastPocketSelections[pocket], 0, _pockets[pocket].Count) == LastPocketSelections[pocket];
    }

    public void SetLastViewedIndex(int pocket, int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pocket, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pocket, PocketCount);
        if (value <= _pockets[pocket].Count)
        {
            LastPocketSelections[pocket] = value;
        }
    }

    public int GetQuantity(Name item)
    {
        if (!Item.TryGet(item, out var itemData))
        {
            return 0;
        }

        var pocket = itemData.Pocket;
        return ItemStorageHelper.GetQuantity(_pockets[pocket], item);
    }

    public bool Has(Name item, int quantity = 1) => GetQuantity(item) >= quantity;

    public bool CanAdd(Name item, int quantity = 1)
    {
        if (!Item.TryGet(item, out var itemData))
        {
            return false;
        }

        var pocket = itemData.Pocket;
        var maxSize = GetMaxPocketSize(pocket) ?? _pockets[pocket].Count + 1;
        var gameSettings = GameGlobal.GameSettings;
        var result = ItemStorageHelper.CanAdd(_pockets[pocket], maxSize, gameSettings.BagMaxPerSlot, item, quantity);
        if (result && gameSettings.BagPockets[pocket - 1].AutoSort)
        {
            _pockets[pocket].Sort((a, b) => Item.IndexOf(a.Item).CompareTo(Item.IndexOf(b.Item)));
        }

        return result;
    }

    public bool Add(Name item, int quantity = 1)
    {
        if (!Item.TryGet(item, out var itemData))
        {
            return false;
        }

        var pocket = itemData.Pocket;
        var maxSize = GetMaxPocketSize(pocket) ?? _pockets[pocket].Count + 1;
        return ItemStorageHelper.Add(_pockets[pocket], maxSize, GameGlobal.GameSettings.BagMaxPerSlot, item, quantity);
    }

    public bool AddAll(Name item, int quantity = 1)
    {
        return CanAdd(item, quantity) && Add(item, quantity);
    }

    public bool CanRemove(Name item, int quantity = 1) => Has(item, quantity);

    public bool Remove(Name item, int quantity = 1)
    {
        if (!Item.TryGet(item, out var itemData))
        {
            return false;
        }

        var pocket = itemData.Pocket;
        return ItemStorageHelper.Remove(_pockets[pocket], item, quantity);
    }

    public bool RemoveAll(Name item, int quantity = 1)
    {
        return CanRemove(item, quantity) && Remove(item, quantity);
    }

    public bool ReplaceItem(Name oldItem, Name newItem)
    {
        if (!Item.TryGet(oldItem, out var oldItemData) || !Item.TryGet(newItem, out var newItemData))
        {
            return false;
        }

        var pocket = oldItemData.Pocket;
        var result = false;
        for (var i = 0; i < _pockets[pocket].Count; i++)
        {
            var item = _pockets[pocket][i];
            if (item.Item != oldItem)
                continue;

            _pockets[pocket][i] = item with { Item = newItem };
            result = true;
        }

        return result;
    }

    public bool IsRegistered(Name item) => RegisteredItems.Contains(item);

    public void Register(Name item)
    {
        if (!Item.Exists(item))
            return;

        if (!RegisteredItems.Contains(item))
            RegisteredItems.Add(item);
    }

    public void Unregister(Name item)
    {
        RegisteredItems.Remove(item);
    }

    private static int? GetMaxPocketSize(int pocket)
    {
        var bagPockets = GameGlobal.GameSettings.BagPockets;
        var pocketIndex = pocket - 1;
        if (pocketIndex < 0 || pocketIndex >= bagPockets.Length)
            return null;

        return bagPockets[pocketIndex].Size;
    }
}
