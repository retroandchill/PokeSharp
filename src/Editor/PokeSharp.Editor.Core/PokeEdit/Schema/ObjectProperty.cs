using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public interface IObjectProperty : IProperty
{
    ObjectTypeRef ObjectType { get; }
    IEditableType TypeSchema { get; }
}

public sealed class ObjectProperty<TOwner, TValue>(Name name, ObjectTypeRef typeRef, Func<TOwner, TValue> getter)
    : Property<TOwner, TValue>(name, getter),
        IObjectProperty
    where TOwner : class, IEditableEntity<TOwner>
    where TValue : class, IEditableEntity<TValue>
{
    public override EditableTypeRef Type => ObjectType;
    public ObjectTypeRef ObjectType { get; } = typeRef;
    public IEditableType TypeSchema => TValue.Type;
}
