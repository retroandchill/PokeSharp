using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableScalarPropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with,
    TValue? defaultValue
) : EditablePropertyBuilder<TOwner, TValue>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
{
    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableScalarProperty<TOwner, TValue>(this);
    }
}
