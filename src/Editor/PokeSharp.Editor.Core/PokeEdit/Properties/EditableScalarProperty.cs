using System.Numerics;
using System.Text.Json;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableScalarProperty<TRoot, TValue>(EditableScalarPropertyBuilder<TRoot, TValue> builder)
    : EditablePropertyBase<TRoot, TValue>(builder)
    where TRoot : notnull
{
    public override FieldDefinition GetDefinition(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        JsonSerializerOptions? options = null
    )
    {
        if (path.Length != 0)
        {
            throw new InvalidOperationException(
                $"Cannot traverse into scalar property {Name}, path still has {path.Length} segment(s)."
            );
        }

        return EditableProperty.CreateDefinitionForScalar(
            Get(root),
            DefaultValue,
            new PropertySegment(Name),
            this,
            options: options
        );
    }

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
}
