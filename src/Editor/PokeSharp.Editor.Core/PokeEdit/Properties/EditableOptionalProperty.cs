using System.Text.Json;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableOptionalProperty : IEditableProperty
{
    IEditableType? InnerType { get; }
    Type InnerClrType { get; }
}

public interface IEditableOptionalProperty<TRoot, TValue> : IEditableProperty<TRoot, TValue>, IEditableOptionalProperty
    where TRoot : notnull
{
    bool IsSet(TRoot root);
    TRoot Reset(TRoot root);
}

public interface IEditableOptionalValueProperty<TRoot, TValue> : IEditableOptionalProperty<TRoot, TValue?>
    where TRoot : notnull
    where TValue : struct
{
    new IEditableType<TValue>? InnerType { get; }
}

public interface IEditableOptionalReferenceProperty<TRoot, TValue> : IEditableOptionalProperty<TRoot, TValue?>
    where TRoot : notnull
    where TValue : class
{
    new IEditableType<TValue>? InnerType { get; }
}

public sealed class EditableOptionalValueProperty<TRoot, TValue>(
    EditableOptionalValuePropertyBuilder<TRoot, TValue> builder,
    ModelBuildCache cache
) : EditablePropertyBase<TRoot, TValue?>(builder), IEditableOptionalValueProperty<TRoot, TValue>
    where TRoot : notnull
    where TValue : struct
{
    IEditableType? IEditableOptionalProperty.InnerType => InnerType;

    public IEditableType<TValue>? InnerType => cache.GetOrBuildType(builder.TargetInnerType);
    public Type InnerClrType => typeof(TValue);

    public override TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length == 0)
        {
            return edit switch
            {
                SetValueEdit set => With(root, set.NewValue.Deserialize<TValue>(options)),
                OptionalResetEdit => Reset(root),
                _ => throw new InvalidOperationException($"Edit {edit} is not valid for scalar property {Name}"),
            };
        }

        if (InnerType is null)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into scalar property {Name}, path still has {path.Length} segment(s)."
            );
        }

        var currentValue = Get(root);
        return currentValue is not null
            ? With(root, InnerType.ApplyEdit(currentValue.Value, path, edit, options))
            : throw new InvalidOperationException($"Cannot traverse into optional property {Name}, no value set.");
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
        if (EqualityComparer<TValue?>.Default.Equals(oldValue, newValue))
            return;

        if (oldValue is not null && newValue is null)
        {
            edits.Add(new OptionalResetEdit { Path = new FieldPath(basePath.Segments.Add(new PropertySegment(Name))) });
        }
        else
        {
            edits.Add(
                new SetValueEdit
                {
                    NewValue = JsonSerializer.SerializeToNode(newValue, options).RequireNonNull(),
                    Path = new FieldPath(basePath.Segments.Add(new PropertySegment(Name))),
                }
            );
        }
    }

    public bool IsSet(TRoot root)
    {
        return Get(root) is not null;
    }

    public TRoot Reset(TRoot root)
    {
        return With(root, null);
    }
}

public sealed class EditableOptionalReferenceProperty<TRoot, TValue>(
    EditableOptionalReferencePropertyBuilder<TRoot, TValue> builder,
    ModelBuildCache cache
) : EditablePropertyBase<TRoot, TValue?>(builder), IEditableOptionalReferenceProperty<TRoot, TValue>
    where TRoot : notnull
    where TValue : class
{
    IEditableType? IEditableOptionalProperty.InnerType => InnerType;

    public IEditableType<TValue>? InnerType => cache.GetOrBuildType(builder.TargetInnerType);
    public Type InnerClrType => typeof(TValue);

    public override TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length == 0)
        {
            return edit switch
            {
                SetValueEdit set => With(root, set.NewValue.Deserialize<TValue>(options).RequireNonNull()),
                OptionalResetEdit => Reset(root),
                _ => throw new InvalidOperationException($"Edit {edit} is not valid for scalar property {Name}"),
            };
        }

        if (InnerType is null)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into scalar property {Name}, path still has {path.Length} segment(s)."
            );
        }

        var currentValue = Get(root);
        return currentValue is not null
            ? With(root, InnerType.ApplyEdit(currentValue, path, edit, options))
            : throw new InvalidOperationException($"Cannot traverse into optional property {Name}, no value set.");
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
        if (EqualityComparer<TValue?>.Default.Equals(oldValue, newValue))
            return;

        if (oldValue is not null && newValue is null)
        {
            edits.Add(new OptionalResetEdit { Path = new FieldPath(basePath.Segments.Add(new PropertySegment(Name))) });
        }
        else
        {
            edits.Add(
                new SetValueEdit
                {
                    NewValue = JsonSerializer.SerializeToNode(newValue, options).RequireNonNull(),
                    Path = new FieldPath(basePath.Segments.Add(new PropertySegment(Name))),
                }
            );
        }
    }

    public bool IsSet(TRoot root)
    {
        return Get(root) is not null;
    }

    public TRoot Reset(TRoot root)
    {
        return With(root, null);
    }
}
