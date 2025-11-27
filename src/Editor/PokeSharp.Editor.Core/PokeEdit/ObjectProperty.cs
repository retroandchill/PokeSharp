using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public interface IObjectProperty : IProperty
{
    IEditableType ElementType { get; }
}

public class ObjectProperty<TObject>(
    Name name,
    IEditableType owner,
    Func<object, TObject> getter,
    Action<object, TObject> setter
) : BasicProperty<TObject>(name, owner, getter, setter), IObjectProperty
    where TObject : class, IEditableEntity<TObject>
{
    public override PropertyKind Kind => PropertyKind.Object;

    public IEditableType ElementType => TObject.Type;
}
