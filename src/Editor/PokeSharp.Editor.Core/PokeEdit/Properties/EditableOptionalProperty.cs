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

    public override TRoot ApplyEdit(TRoot root, DiffNode diff, JsonSerializerOptions? options = null)
    {
        switch (diff)
        {
            case ValueSetNode set:
                return With(root, set.NewValue.Deserialize<TValue?>(options));
            case ValueResetNode:
                return Reset(root);
            case ObjectDiffNode obj when InnerType is not null:
                var currentValue = Get(root);
                return currentValue is not null
                    ? With(root, InnerType.ApplyEdit(currentValue.Value, obj, options))
                    : throw new InvalidOperationException(
                        $"Cannot traverse into optional property {Name}, no value set."
                    );
            default:
                throw new InvalidOperationException($"Invalid diff node type: {diff.GetType().Name}");
        }
    }

    public override DiffNode? Diff(TRoot oldRoot, TRoot newRoot, JsonSerializerOptions? options = null)
    {
        var oldValue = Get(oldRoot);
        var newValue = Get(newRoot);
        if (oldValue is null)
        {
            return newValue is not null ? new ValueSetNode(JsonSerializer.SerializeToNode(newValue.Value, options).RequireNonNull()) : null;
        }
        
        if (newValue is null)
        {
            return new ValueResetNode();
        }

        if (InnerType is null)
        {
            return new ValueSetNode(JsonSerializer.SerializeToNode(newValue.Value, options).RequireNonNull());
        }
        
        return InnerType.Diff(oldValue.Value, newValue.Value, options);
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

    public override TRoot ApplyEdit(TRoot root, DiffNode diff, JsonSerializerOptions? options = null)
    {
        switch (diff)
        {
            case ValueSetNode set:
                return With(root, set.NewValue.Deserialize<TValue?>(options));
            case ValueResetNode:
                return Reset(root);
            case ObjectDiffNode obj when InnerType is not null:
                var currentValue = Get(root);
                return currentValue is not null
                    ? With(root, InnerType.ApplyEdit(currentValue, obj, options))
                    : throw new InvalidOperationException(
                        $"Cannot traverse into optional property {Name}, no value set."
                    );
            default:
                throw new InvalidOperationException($"Invalid diff node type: {diff.GetType().Name}");
        }
    }

    public override DiffNode? Diff(TRoot oldRoot, TRoot newRoot, JsonSerializerOptions? options = null)
    {
        var oldValue = Get(oldRoot);
        var newValue = Get(newRoot);
        if (oldValue is null)
        {
            return newValue is not null ? new ValueSetNode(JsonSerializer.SerializeToNode(newValue, options).RequireNonNull()) : null;
        }
        
        if (newValue is null)
        {
            return new ValueResetNode();
        }

        if (InnerType is null)
        {
            return new ValueSetNode(JsonSerializer.SerializeToNode(newValue, options).RequireNonNull());
        }
        
        return InnerType.Diff(oldValue, newValue, options);
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
