using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IEnumProperty : IProperty
{
    EnumTypeRef EnumType { get; }
}

public sealed class EnumProperty<TOwner, TEnum>(Name name, EnumTypeRef typeRef, Func<TOwner, TEnum> getter)
    : Property<TOwner, TEnum>(name, getter),
        IEnumProperty
    where TOwner : class, IEditableEntity<TOwner>
    where TEnum : unmanaged, Enum
{
    public override EditableTypeRef Type => EnumType;
    public EnumTypeRef EnumType { get; } = typeRef;
}
