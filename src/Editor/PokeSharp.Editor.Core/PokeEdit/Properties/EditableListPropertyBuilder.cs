using System.Collections.Immutable;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableListPropertyBuilder<TOwner, TItem>(
    EditableTypeBuilder<TOwner> typeBuilder,
    string name,
    Func<TOwner, ImmutableArray<TItem>> get,
    Func<TOwner, ImmutableArray<TItem>, TOwner> with,
    ImmutableArray<TItem> defaultValue
) : EditablePropertyBuilder<TOwner, ImmutableArray<TItem>>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
    where TItem : notnull
{
    internal EditableTypeBuilder<TItem>? TargetItemType => TypeBuilder.ModelBuilder.GetBuildIfPossible<TItem>();

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableListProperty<TOwner, TItem>(this, cache);
    }
}
