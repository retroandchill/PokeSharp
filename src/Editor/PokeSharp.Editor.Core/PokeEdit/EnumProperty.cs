using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public sealed class EnumProperty<TEnum>(
    Name name,
    IEditableType owner,
    Func<object, TEnum> getter,
    Action<object, TEnum>? setter = null
) : BasicProperty<TEnum>(name, owner, getter, setter)
    where TEnum : unmanaged, Enum
{
    public override PropertyKind Kind => PropertyKind.Enum;
}
