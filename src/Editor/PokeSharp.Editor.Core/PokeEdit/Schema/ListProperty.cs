using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IListProperty : IProperty
{
    ListTypeRef ListType { get; }

    int GetCount(object target);
    object? GetItem(object target, int index);
    void SetItem(object target, int index, object? value);
    void AddItem(object target, object? value);
    void InsertItem(object target, int index, object? value);
    void RemoveItem(object target, int index);
    void ClearItems(object target);
}

public sealed class ListProperty<TOwner, TValue> : Property<TOwner, List<TValue>>, IListProperty
    where TOwner : class, IEditableEntity<TOwner>
{
    public override EditableTypeRef Type => ListType;
    public ListTypeRef ListType { get; }

    public ListProperty(Name name, ListTypeRef typeRef, Func<TOwner, List<TValue>> getter)
        : base(name, getter)
    {
        ListType = typeRef;
        DefaultValue = () => [];
    }

    private List<TValue> GetListOrThrow(TOwner target) => GetValueOrThrow(Getter(target));

    private static TValue GetElementOrThrow(object? value)
    {
        return value is TValue typedValue
            ? typedValue
            : throw new ArgumentException($"Value must be of type {typeof(TValue)}.");
    }

    public int GetCount(object target)
    {
        return GetListOrThrow(GetOwnerOrThrow(target)).Count;
    }

    public object? GetItem(object target, int index)
    {
        return GetListOrThrow(GetOwnerOrThrow(target))[index];
    }

    public void SetItem(object target, int index, object? value)
    {
        GetListOrThrow(GetOwnerOrThrow(target))[index] = GetElementOrThrow(value);
    }

    public void AddItem(object target, object? value)
    {
        GetListOrThrow(GetOwnerOrThrow(target)).Add(GetElementOrThrow(value));
    }

    public void InsertItem(object target, int index, object? value)
    {
        GetListOrThrow(GetOwnerOrThrow(target)).Insert(index, GetElementOrThrow(value));
    }

    public void RemoveItem(object target, int index)
    {
        GetListOrThrow(GetOwnerOrThrow(target)).RemoveAt(index);
    }

    public void ClearItems(object target)
    {
        GetListOrThrow(GetOwnerOrThrow(target)).Clear();
    }
}
