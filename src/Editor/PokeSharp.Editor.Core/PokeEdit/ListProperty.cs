using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IListProperty : IProperty
{
    Type ElementType { get; }

    int GetCount(object target);
    object? GetItem(object target, int index);
    void SetItem(object target, int index, object? value);
    void AddItem(object target, object? value);
    void RemoveItem(object target, int index);
    void Clear(object target);
}

public sealed class ListProperty<TElement>(
    Name name,
    IEditableType owner,
    Func<object, List<TElement>> getter,
    Action<object, List<TElement>>? setter = null
) : BasicProperty<List<TElement>>(name, owner, getter, setter), IListProperty
{
    public override PropertyKind Kind => PropertyKind.List;
    public Type ElementType => typeof(TElement);

    private List<TElement> GetList(object target)
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

        if (value is not IEnumerable<TElement> castValue)
            throw new ArgumentException(
                $"Value for '{Name}' must be an {typeof(IEnumerable<TElement>).Name}.",
                nameof(value)
            );

        Setter(target, castValue.ToList());
    }

    public int GetCount(object target)
    {
        return GetList(target).Count;
    }

    public object? GetItem(object target, int index)
    {
        return GetList(target)[index];
    }

    public void SetItem(object target, int index, object? value)
    {
        if (value is not TElement castValue)
            throw new ArgumentException($"Value for '{Name}' must be an {typeof(TElement).Name}.", nameof(value));

        GetList(target)[index] = castValue;
    }

    public void AddItem(object target, object? value)
    {
        if (value is not TElement castValue)
            throw new ArgumentException($"Value for '{Name}' must be an {typeof(TElement).Name}.", nameof(value));

        GetList(target).Add(castValue);
    }

    public void RemoveItem(object target, int index)
    {
        GetList(target).RemoveAt(index);
    }

    public void Clear(object target)
    {
        GetList(target).Clear();
    }
}
