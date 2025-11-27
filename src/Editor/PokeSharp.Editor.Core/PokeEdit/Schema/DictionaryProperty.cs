using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IDictionaryProperty : IProperty
{
    DictionaryTypeRef DictionaryType { get; }

    int GetCount(object target);
    object? GetItem(object target, object key);
    void SetItem(object target, object key, object? value);
    void AddItem(object target, object key, object? value);
    void RemoveItem(object target, object key);
    void ClearItems(object target);
}

public sealed class DictionaryProperty<TOwner, TKey, TValue>
    : Property<TOwner, Dictionary<TKey, TValue>>,
        IDictionaryProperty
    where TOwner : class, IEditableEntity<TOwner>
    where TKey : notnull
{
    public override EditableTypeRef Type => DictionaryType;
    public DictionaryTypeRef DictionaryType { get; }

    public DictionaryProperty(Name name, DictionaryTypeRef typeRef, Func<TOwner, Dictionary<TKey, TValue>> getter)
        : base(name, getter)
    {
        DictionaryType = typeRef;
        DefaultValue = () => new Dictionary<TKey, TValue>();
    }

    private Dictionary<TKey, TValue> GetDictionaryOrThrow(TOwner target) => GetValueOrThrow(Getter(target));

    private static TKey GetKeyOrThrow(object key)
    {
        return key is TKey typedKey ? typedKey : throw new ArgumentException($"Key must be of type {typeof(TKey)}.");
    }

    private static TValue GetElementOrThrow(object? value)
    {
        return value is TValue typedValue
            ? typedValue
            : throw new ArgumentException($"Value must be of type {typeof(TValue)}.");
    }

    public int GetCount(object target)
    {
        return GetDictionaryOrThrow(GetOwnerOrThrow(target)).Count;
    }

    public object? GetItem(object target, object key)
    {
        return GetDictionaryOrThrow(GetOwnerOrThrow(target))[GetKeyOrThrow(key)];
    }

    public void SetItem(object target, object key, object? value)
    {
        GetDictionaryOrThrow(GetOwnerOrThrow(target))[GetKeyOrThrow(key)] = GetElementOrThrow(value);
    }

    public void AddItem(object target, object key, object? value)
    {
        GetDictionaryOrThrow(GetOwnerOrThrow(target)).Add(GetKeyOrThrow(key), GetElementOrThrow(value));
    }

    public void RemoveItem(object target, object key)
    {
        GetDictionaryOrThrow(GetOwnerOrThrow(target)).Remove(GetKeyOrThrow(key));
    }

    public void ClearItems(object target)
    {
        GetDictionaryOrThrow(GetOwnerOrThrow(target)).Clear();
    }
}
