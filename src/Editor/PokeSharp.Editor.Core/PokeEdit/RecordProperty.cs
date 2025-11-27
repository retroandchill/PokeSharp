using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit;

public sealed class RecordProperty<TRecord>(
    Name name,
    IEditableType owner,
    Func<object, TRecord> getter,
    Action<object, TRecord>? setter = null
) : BasicProperty<TRecord>(name, owner, getter, setter)
{
    public override PropertyKind Kind => PropertyKind.Record;
}
