using System.Text.Json;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class EditableScalarProperty<TRoot, TValue>(EditableScalarPropertyBuilder<TRoot, TValue> builder)
    : EditablePropertyBase<TRoot, TValue>(builder)
    where TRoot : notnull
{
    public override TRoot ApplyEdit(TRoot root, DiffNode diff, JsonSerializerOptions? options = null)
    {
        return diff is ValueSetNode set
            ? With(root, set.NewValue.Deserialize<TValue>(options).RequireNonNull())
            : throw new InvalidOperationException($"Diff is not valid for scalar property {Name}");
    }

    public override DiffNode? Diff(TRoot oldRoot, TRoot newRoot, JsonSerializerOptions? options = null)
    {
        var oldValue = Get(oldRoot);
        var newValue = Get(newRoot);

        return !EqualityComparer<TValue>.Default.Equals(oldValue, newValue)
            ? new ValueSetNode(JsonSerializer.SerializeToNode(newValue, options).RequireNonNull())
            : null;
    }
}
