using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public abstract class BasicProperty(Name name, IEditableType owner) : IProperty
{
    public Name Name { get; } = name;
    public abstract PropertyKind Kind { get; }
    public abstract Type Type { get; }
    public IEditableType Owner { get; } = owner;
    public abstract bool IsReadOnly { get; }

    public abstract object? GetValue(object target);

    public abstract void SetValue(object target, object? value);
}

public abstract class BasicProperty<TValue>(
    Name name,
    IEditableType owner,
    Func<object, TValue> getter,
    Action<object, TValue>? setter = null
) : BasicProperty(name, owner)
{
    protected Func<object, TValue> Getter { get; } = getter;
    protected Action<object, TValue>? Setter { get; } = setter;
    public sealed override Type Type => typeof(TValue);

    [MemberNotNullWhen(false, nameof(Setter))]
    public sealed override bool IsReadOnly => Setter is null;

    public override object? GetValue(object target)
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

        if (value is not TValue castValue)
            throw new ArgumentException($"Value for '{Name}' must be an {typeof(TValue).Name}.", nameof(value));

        Setter(target, castValue);
    }
}

public sealed class BoolProperty(
    Name name,
    IEditableType owner,
    Func<object, bool> getter,
    Action<object, bool>? setter = null
) : BasicProperty<bool>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Bool;
}

public sealed class SByteProperty(
    Name name,
    IEditableType owner,
    Func<object, sbyte> getter,
    Action<object, sbyte>? setter = null
) : BasicProperty<sbyte>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Int8;
}

public sealed class ShortProperty(
    Name name,
    IEditableType owner,
    Func<object, short> getter,
    Action<object, short>? setter = null
) : BasicProperty<short>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Int16;
}

public sealed class IntProperty(
    Name name,
    IEditableType owner,
    Func<object, int> getter,
    Action<object, int>? setter = null
) : BasicProperty<int>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Int32;
}

public sealed class LongProperty(
    Name name,
    IEditableType owner,
    Func<object, long> getter,
    Action<object, long>? setter = null
) : BasicProperty<long>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Int64;
}

public sealed class ByteProperty(
    Name name,
    IEditableType owner,
    Func<object, byte> getter,
    Action<object, byte>? setter = null
) : BasicProperty<byte>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.UInt8;
}

public sealed class UShortProperty(
    Name name,
    IEditableType owner,
    Func<object, ushort> getter,
    Action<object, ushort>? setter = null
) : BasicProperty<ushort>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.UInt16;
}

public sealed class UIntProperty(
    Name name,
    IEditableType owner,
    Func<object, uint> getter,
    Action<object, uint>? setter = null
) : BasicProperty<uint>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.UInt32;
}

public sealed class ULongProperty(
    Name name,
    IEditableType owner,
    Func<object, ulong> getter,
    Action<object, ulong>? setter = null
) : BasicProperty<ulong>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.UInt64;
}

public sealed class FloatProperty(
    Name name,
    IEditableType owner,
    Func<object, float> getter,
    Action<object, float>? setter = null
) : BasicProperty<float>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Float;
}

public sealed class DoubleProperty(
    Name name,
    IEditableType owner,
    Func<object, double> getter,
    Action<object, double>? setter = null
) : BasicProperty<double>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Double;
}

public sealed class NameProperty(
    Name name,
    IEditableType owner,
    Func<object, Name> getter,
    Action<object, Name>? setter = null
) : BasicProperty<Name>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Name;
}

public sealed class StringProperty(
    Name name,
    IEditableType owner,
    Func<object, string> getter,
    Action<object, string>? setter = null
) : BasicProperty<string>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.String;
}

public sealed class TextProperty(
    Name name,
    IEditableType owner,
    Func<object, Text> getter,
    Action<object, Text>? setter = null
) : BasicProperty<Text>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Text;
}
