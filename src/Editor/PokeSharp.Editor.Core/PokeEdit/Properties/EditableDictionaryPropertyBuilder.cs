using System.Collections.Immutable;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableDictionaryPropertyBuilder<TOwner, TKey, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    string name,
    Func<TOwner, ImmutableDictionary<TKey, TValue>> get,
    Func<TOwner, ImmutableDictionary<TKey, TValue>, TOwner> with,
    ImmutableDictionary<TKey, TValue>? defaultValue
) : EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
    where TKey : notnull
    where TValue : notnull
{
    internal EditableTypeBuilder<TKey>? TargetKeyType => TypeBuilder.ModelBuilder.GetBuildIfPossible<TKey>();
    internal EditableTypeBuilder<TValue>? TargetValueType => TypeBuilder.ModelBuilder.GetBuildIfPossible<TValue>();

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableDictionaryProperty<TOwner, TKey, TValue>(this, cache);
    }
}
