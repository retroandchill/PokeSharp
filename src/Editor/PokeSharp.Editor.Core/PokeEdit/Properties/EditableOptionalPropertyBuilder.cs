using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableOptionalValuePropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    string name,
    Func<TOwner, TValue?> get,
    Func<TOwner, TValue?, TOwner> with,
    TValue? defaultValue
) : EditablePropertyBuilder<TOwner, TValue?>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
    where TValue : struct
{
    internal EditableTypeBuilder<TValue>? TargetInnerType => TypeBuilder.ModelBuilder.GetBuildIfPossible<TValue>();

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableOptionalValueProperty<TOwner, TValue>(this, cache);
    }
}

public sealed class EditableOptionalReferencePropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    string name,
    Func<TOwner, TValue?> get,
    Func<TOwner, TValue?, TOwner> with,
    TValue? defaultValue
) : EditablePropertyBuilder<TOwner, TValue?>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
    where TValue : class
{
    internal EditableTypeBuilder<TValue>? TargetInnerType => TypeBuilder.ModelBuilder.GetBuildIfPossible<TValue>();

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableOptionalReferenceProperty<TOwner, TValue>(this, cache);
    }
}
