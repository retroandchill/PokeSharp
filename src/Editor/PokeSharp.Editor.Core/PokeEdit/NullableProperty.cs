using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface INullableProperty : IProperty
{
    Type ElementType { get; }

    bool IsNull(object target);
}

public sealed class NullableProperty<TStruct>(
    Name name,
    IEditableType owner,
    Func<object, TStruct?> getter,
    Action<object, TStruct?>? setter = null
) : BasicProperty<TStruct?>(name, owner, getter, setter), INullableProperty
    where TStruct : struct
{
    public override PropertyKind Kind => PropertyKind.Nullable;
    public Type ElementType => typeof(TStruct);

    public bool IsNull(object target) => GetValue(target) is null;

    public override void SetValue(object target, object? value)
    {
        if (IsReadOnly)
            throw new InvalidOperationException($"Property '{Name}' is read-only.");

        if (!Owner.IsInstanceOfType(target))
            throw new ArgumentException($"Target is not a {Owner}", nameof(target));

        if (value is null)
        {
            Setter(target, null);
            return;
        }

        if (value is not TStruct castValue)
            throw new ArgumentException($"Value for '{Name}' must be an {typeof(TStruct).Name}.", nameof(value));

        Setter(target, castValue);
    }
}
