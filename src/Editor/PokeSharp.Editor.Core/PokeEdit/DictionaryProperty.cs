using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IDictionaryProperty : IProperty
{
    Type KeyType { get; }
    Type ValueType { get; }

    int GetCount(object target);
    object? GetItem(object target, object key);
    void SetItem(object target, object key, object? value);
    void AddItem(object target, object key, object? value);
    void RemoveItem(object target, object key);
    void Clear(object target);
}

public sealed class DictionaryProperty<TKey, TValue>(
    Name name,
    IEditableType owner,
    Func<object, Dictionary<TKey, TValue>> getter,
    Action<object, Dictionary<TKey, TValue>>? setter = null
) : BasicProperty<Dictionary<TKey, TValue>>(name, owner, getter, setter), IDictionaryProperty
    where TKey : notnull
{
    public override PropertyKind Kind => PropertyKind.Dictionary;
    public Type KeyType => typeof(TKey);
    public Type ValueType => typeof(TValue);

    private Dictionary<TKey, TValue> GetDictionary(object target)
    {
        return Owner.IsInstanceOfType(target)
            ? Getter(target)
            : throw new ArgumentException($"Target is not a {Owner}", nameof(target));
    }

    public override void SetValue(object target, object? value)
    {
        if (IsReadOnly)
            throw new InvalidOperationException($"Property '{Name}' is read-only.");

        if (!Owner.IsInstanceOfType(target))
            throw new ArgumentException($"Target is not a {Owner}", nameof(target));

        if (value is not IEnumerable<KeyValuePair<TKey, TValue>> castValue)
            throw new ArgumentException(
                $"Value for '{Name}' must be an {typeof(IEnumerable<KeyValuePair<TKey, TValue>>).Name}.",
                nameof(value)
            );

        Setter(target, castValue.ToDictionary());
    }

    public int GetCount(object target)
    {
        return GetDictionary(target).Count;
    }

    public object? GetItem(object target, object key)
    {
        return key is TKey castKey
            ? GetDictionary(target)[castKey]
            : throw new ArgumentException($"Key for '{Name}' must be an {typeof(TKey).Name}.", nameof(key));
    }

    public void SetItem(object target, object key, object? value)
    {
        if (key is not TKey castKey)
        {
            throw new ArgumentException($"Key for '{Name}' must be an {typeof(TKey).Name}.", nameof(key));
        }

        if (value is not TValue castValue)
        {
            throw new ArgumentException($"Value for '{Name}' must be an {typeof(TValue).Name}.", nameof(value));
        }

        GetDictionary(target)[castKey] = castValue;
    }

    public void AddItem(object target, object key, object? value)
    {
        if (key is not TKey castKey)
        {
            throw new ArgumentException($"Key for '{Name}' must be an {typeof(TKey).Name}.", nameof(key));
        }

        if (value is not TValue castValue)
        {
            throw new ArgumentException($"Value for '{Name}' must be an {typeof(TValue).Name}.", nameof(value));
        }

        GetDictionary(target).Add(castKey, castValue);
    }

    public void RemoveItem(object target, object key)
    {
        if (key is not TKey castKey)
        {
            throw new ArgumentException($"Key for '{Name}' must be an {typeof(TKey).Name}.", nameof(key));
        }

        GetDictionary(target).Remove(castKey);
    }

    public void Clear(object target)
    {
        GetDictionary(target).Clear();
    }
}
