using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableObjectPropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    string name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with,
    TValue? defaultValue
) : EditablePropertyBuilder<TOwner, TValue>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
    where TValue : notnull
{
    internal EditableTypeBuilder<TValue> TargetType => TypeBuilder.ModelBuilder.GetOrCreateBuilder<TValue>();

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableObjectProperty<TOwner, TValue>(this, cache);
    }
}
