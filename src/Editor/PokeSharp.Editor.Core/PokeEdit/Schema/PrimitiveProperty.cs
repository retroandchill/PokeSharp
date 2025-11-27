using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IPrimitiveProperty : IProperty
{
    PrimitiveTypeRef PrimitiveType { get; }
}

public sealed class PrimitiveProperty<TOwner, TValue>(Name name, PrimitiveTypeRef typeRef, Func<TOwner, TValue> getter)
    : Property<TOwner, TValue>(name, getter),
        IPrimitiveProperty
    where TOwner : class, IEditableEntity<TOwner>
{
    public override EditableTypeRef Type => PrimitiveType;
    public PrimitiveTypeRef PrimitiveType { get; } = typeRef;
}
