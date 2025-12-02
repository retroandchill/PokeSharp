using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace PokeSharp.Core.Collections.Immutable;

public class ImmutableOrderedDictionary<TKey, TValue> : IImmutableDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly ImmutableDictionary<TKey, TValue> _dictionary;

    public ImmutableArray<TKey> Keys { get; }
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;
    public IEnumerable<TValue> Values => Keys.Select(key => _dictionary[key]);

    public int Count => _dictionary.Count;
    public bool IsEmpty => _dictionary.IsEmpty;

    public TValue this[TKey key] => _dictionary[key];

    public IEqualityComparer<TKey> KeyComparer => _dictionary.KeyComparer;
    public IEqualityComparer<TValue> ValueComparer => _dictionary.ValueComparer;

    private ImmutableOrderedDictionary(ImmutableDictionary<TKey, TValue> dictionary, ImmutableArray<TKey> keys)
    {
        _dictionary = dictionary;
        Keys = keys;
    }

    public bool Contains(KeyValuePair<TKey, TValue> pair) => _dictionary.Contains(pair);

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    public bool ContainsValue(TValue value) => _dictionary.ContainsValue(value);

    public KeyValuePair<TKey, TValue> GetAt(int index)
    {
        var key = Keys[index];
        return new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
    }

    public bool TryGetKey(TKey equalKey, out TKey actualKey) => _dictionary.TryGetKey(equalKey, out actualKey);

    public bool TryGetValue(TKey key, [NotNullWhen(false)] out TValue? value) =>
        _dictionary.TryGetValue(key, out value);

    public int IndexOf(TKey key) => Keys.IndexOf(key);

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ImmutableOrderedDictionary<TKey, TValue> SetItem(TKey key, TValue value)
    {
        var newDictionary = _dictionary.SetItem(key, value);
        if (ReferenceEquals(_dictionary, newDictionary))
        {
            return this;
        }

        return _dictionary.ContainsKey(key)
            ? new ImmutableOrderedDictionary<TKey, TValue>(newDictionary, Keys)
            : new ImmutableOrderedDictionary<TKey, TValue>(newDictionary, Keys.Add(key));
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItem(TKey key, TValue value)
    {
        return SetItem(key, value);
    }

    public ImmutableOrderedDictionary<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> values)
    {
        var collection = values as IReadOnlyCollection<KeyValuePair<TKey, TValue>> ?? values.ToArray();
        var newDictionary = _dictionary.SetItems(collection);
        if (ReferenceEquals(_dictionary, newDictionary))
        {
            return this;
        }

        return new ImmutableOrderedDictionary<TKey, TValue>(
            newDictionary,
            Keys.AddRange(collection.Select(x => x.Key).Where(x => !_dictionary.ContainsKey(x)))
        );
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItems(
        IEnumerable<KeyValuePair<TKey, TValue>> items
    )
    {
        return SetItems(items);
    }

    public ImmutableOrderedDictionary<TKey, TValue> SetAt(int index, TValue value)
    {
        var key = Keys[index];
        return new ImmutableOrderedDictionary<TKey, TValue>(_dictionary.SetItem(key, value), Keys);
    }

    public ImmutableOrderedDictionary<TKey, TValue> SetAt(int index, TKey key, TValue value)
    {
        var existingKey = Keys[index];
        if (KeyComparer.Equals(existingKey, key))
        {
            return SetAt(index, value);
        }

        var builder = _dictionary.ToBuilder();
        builder.Remove(existingKey);
        builder.Add(key, value);

        return new ImmutableOrderedDictionary<TKey, TValue>(builder.ToImmutable(), Keys.SetItem(index, key));
    }

    public ImmutableOrderedDictionary<TKey, TValue> Add(TKey key, TValue value)
    {
        var newDictionary = _dictionary.Add(key, value);
        return ReferenceEquals(_dictionary, newDictionary)
            ? this
            : new ImmutableOrderedDictionary<TKey, TValue>(newDictionary, Keys.Add(key));
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        return Add(key, value);
    }

    public ImmutableOrderedDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
    {
        var itemsCollection = items as IReadOnlyCollection<KeyValuePair<TKey, TValue>> ?? items.ToArray();
        var newDictionary = _dictionary.AddRange(itemsCollection);
        if (ReferenceEquals(_dictionary, newDictionary))
        {
            return this;
        }

        return new ImmutableOrderedDictionary<TKey, TValue>(
            newDictionary,
            Keys.AddRange(itemsCollection.Select(x => x.Key).Where(x => !_dictionary.ContainsKey(x)))
        );
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.AddRange(
        IEnumerable<KeyValuePair<TKey, TValue>> items
    )
    {
        return AddRange(items);
    }

    public ImmutableOrderedDictionary<TKey, TValue> Insert(int index, TKey key, TValue value)
    {
        var newDictionary = _dictionary.Add(key, value);
        return ReferenceEquals(_dictionary, newDictionary)
            ? this
            : new ImmutableOrderedDictionary<TKey, TValue>(newDictionary, Keys.Insert(index, key));
    }

    public ImmutableOrderedDictionary<TKey, TValue> Remove(TKey key)
    {
        var newDictionary = _dictionary.Remove(key);
        return ReferenceEquals(_dictionary, newDictionary)
            ? this
            : new ImmutableOrderedDictionary<TKey, TValue>(newDictionary, Keys.Remove(key));
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Remove(TKey key)
    {
        return Remove(key);
    }

    public ImmutableOrderedDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
    {
        var newDictionary = _dictionary.RemoveRange(keys);
        if (ReferenceEquals(_dictionary, newDictionary))
            return this;

        return new ImmutableOrderedDictionary<TKey, TValue>(
            newDictionary,
            Keys.RemoveAll(x => !newDictionary.ContainsKey(x))
        );
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.RemoveRange(IEnumerable<TKey> keys)
    {
        return RemoveRange(keys);
    }

    public ImmutableOrderedDictionary<TKey, TValue> Clear()
    {
        return IsEmpty
            ? this
            : new ImmutableOrderedDictionary<TKey, TValue>(_dictionary.Clear(), ImmutableArray<TKey>.Empty);
    }

    IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Clear() => Clear();

    public ImmutableOrderedDictionary<TKey, TValue> WithComparers(
        IEqualityComparer<TKey>? keyComparer,
        IEqualityComparer<TValue>? valueComparer
    )
    {
        var newDictionary = _dictionary.WithComparers(keyComparer, valueComparer);
        if (ReferenceEquals(newDictionary, _dictionary))
            return this;

        if (ReferenceEquals(newDictionary.KeyComparer, _dictionary.KeyComparer))
        {
            return new ImmutableOrderedDictionary<TKey, TValue>(newDictionary, Keys);
        }

        var keyHashSet = new HashSet<TKey>(Keys.Length, newDictionary.KeyComparer);
        var keyBuilder = ImmutableArray.CreateBuilder<TKey>(Keys.Length);
        foreach (var key in Keys.Where(key => keyHashSet.Add(key)))
        {
            keyBuilder.Add(key);
        }

        return new ImmutableOrderedDictionary<TKey, TValue>(newDictionary, keyBuilder.MoveToImmutable());
    }

    public ImmutableOrderedDictionary<TKey, TValue> WithComparers(IEqualityComparer<TKey>? keyComparer)
    {
        return WithComparers(keyComparer, _dictionary.ValueComparer);
    }

    public Builder ToBuilder() => throw new NotImplementedException();

    // TODO: Implement builder
    public class Builder { }

    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly ImmutableOrderedDictionary<TKey, TValue> _dictionary;
        private int _index = -1;

        internal Enumerator(ImmutableOrderedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        public KeyValuePair<TKey, TValue> Current => _dictionary.GetAt(_index);
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_index >= _dictionary.Count)
                return false;

            _index++;
            return true;
        }

        public void Reset()
        {
            _index = -1;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}
