using System.Text.Json;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableScalarProperty<TRoot, TValue>(EditableScalarPropertyBuilder<TRoot, TValue> builder)
    : EditablePropertyBase<TRoot, TValue>(builder)
    where TRoot : notnull
{
    public override TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length != 0)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into scalar property {Name}, path still has {path.Length} segment(s)."
            );
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

        if (!EqualityComparer<TValue>.Default.Equals(oldValue, newValue))
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
}
