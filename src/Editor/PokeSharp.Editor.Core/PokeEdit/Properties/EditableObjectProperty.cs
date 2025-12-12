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

    public override TRoot ApplyEdit(TRoot root, DiffNode diff, JsonSerializerOptions? options = null)
    {
        return diff switch
        {
            ValueSetNode set => With(root, set.NewValue.Deserialize<TValue>(options).RequireNonNull()),
            ObjectDiffNode obj => With(root, Type.ApplyEdit(Get(root), obj, options)),
            _ => throw new InvalidOperationException($"Invalid diff node type: {diff.GetType().Name}"),
        };
    }

    public override DiffNode? Diff(TRoot oldRoot, TRoot newRoot, JsonSerializerOptions? options = null)
    {
        return Type.Diff(Get(oldRoot), Get(newRoot), options);
    }
}
