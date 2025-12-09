using System.Text.Json;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableObjectProperty : IEditableProperty
{
    IEditableType Type { get; }
}

public interface IEditableObjectProperty<TRoot, TValue> : IEditableProperty<TRoot, TValue>, IEditableObjectProperty
    where TRoot : notnull
    where TValue : notnull
{
    new IEditableType<TValue> Type { get; }
}

public sealed class EditableObjectProperty<TRoot, TValue>(
    EditableObjectPropertyBuilder<TRoot, TValue> builder,
    ModelBuildCache cache
) : EditablePropertyBase<TRoot, TValue>(builder), IEditableObjectProperty<TRoot, TValue>
    where TRoot : notnull
    where TValue : notnull
{
    IEditableType IEditableObjectProperty.Type => Type;
    public IEditableType<TValue> Type { get; } = cache.GetOrBuildType(builder.TargetType);

    public override TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length != 0)
        {
            return With(root, Type.ApplyEdit(Get(root), path, edit, options));
        }

        return edit is SetValueEdit set
            ? With(root, set.NewValue.Deserialize<TValue>(options).RequireNonNull())
            : throw new InvalidOperationException($"Edit {edit} is not valid for scalar property {Name}");
    }

    public override void CollectDiffs(
        TRoot oldRoot,
        TRoot newRoot,
        List<FieldEdit> edits,
        FieldPath basePath,
        JsonSerializerOptions? options = null
    )
    {
        var oldValue = Get(oldRoot);
        var newValue = Get(newRoot);

        if (EqualityComparer<TValue>.Default.Equals(oldValue, newValue))
            return;

        var propertyPath = new FieldPath(basePath.Segments.Add(new PropertySegment(Name)));
        Type.CollectDiffs(oldValue, newValue, edits, propertyPath, options);
    }
}
