using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface INullableProperty : IProperty
{
    NullableTypeRef NullableType { get; }
}

public sealed class NullableProperty<TOwner, TValue>(Name name, NullableTypeRef typeRef, Func<TOwner, TValue?> getter)
    : Property<TOwner, TValue?>(name, getter),
        INullableProperty
    where TOwner : class, IEditableEntity<TOwner>
    where TValue : struct
{
    public override EditableTypeRef Type => NullableType;
    public NullableTypeRef NullableType { get; } = typeRef;
}
