using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IProperty
{
    Name Name { get; }
    IEditableType Owner { get; }
    EditableTypeRef Type { get; }
    Type ClrType { get; }
    bool IsReadOnly { get; }

    object? DefaultValue { get; }
    object? GetValue(object target);
    void SetValue(object target, object? value);
    void ResetValue(object target);
}

public abstract class Property<TOwner, TValue>(Name name, Func<TOwner, TValue> getter) : IProperty
    where TOwner : class, IEditableEntity<TOwner>
{
    public Name Name { get; } = name;
    public IEditableType Owner => TOwner.Type;
    public abstract EditableTypeRef Type { get; }
    public Type ClrType => typeof(TValue);

    [MemberNotNullWhen(false, nameof(Setter))]
    public bool IsReadOnly => Setter is null;

    public Func<TOwner, TValue> Getter { get; } = getter;
    public Action<TOwner, TValue>? Setter { get; init; }
    public Func<TValue> DefaultValue { get; init; } = () => default!;

    object? IProperty.DefaultValue => DefaultValue();

    protected static TOwner GetOwnerOrThrow(object target) =>
        target as TOwner ?? throw new ArgumentException($"Target must be of type {typeof(TOwner)}.");

    protected TValue GetValueOrThrow(object? value)
    {
        return value switch
        {
            null => throw new ArgumentNullException(nameof(value)),
            TValue typedValue => typedValue,
            _ => throw new ArgumentException($"Value must be of type {typeof(TValue)}."),
        };
    }

    public virtual object? GetValue(object target)
    {
        return Getter(GetOwnerOrThrow(target));
    }

    public virtual void SetValue(object target, object? value)
    {
        if (IsReadOnly)
            throw new InvalidOperationException($"Property '{Name}' is read-only.");

        Setter(GetOwnerOrThrow(target), GetValueOrThrow(value));
    }

    public virtual void ResetValue(object target)
    {
        if (IsReadOnly)
            throw new InvalidOperationException($"Property '{Name}' is read-only.");

        Setter(GetOwnerOrThrow(target), DefaultValue());
    }
}
