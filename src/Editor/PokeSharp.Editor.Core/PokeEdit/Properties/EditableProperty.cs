using System.Collections.Immutable;
using System.Text.Json;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableProperty : IEditableMember
{
    Type ClrType { get; }
}

public interface IEditableProperty<TRoot> : IEditableProperty
    where TRoot : notnull
{
    TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    );

    void CollectDiffs(
        TRoot oldRoot,
        TRoot newRoot,
        List<FieldEdit> edits,
        FieldPath basePath,
        JsonSerializerOptions? options = null
    );
}

public interface IEditableProperty<TRoot, TValue> : IEditableProperty<TRoot>
    where TRoot : notnull
{
    TValue? DefaultValue { get; }

    TValue Get(TRoot root);
    TRoot With(TRoot root, TValue value);
}

public abstract class EditablePropertyBase<TRoot, TValue>(EditablePropertyBuilder<TRoot, TValue> builder)
    : IEditableProperty<TRoot, TValue>
    where TRoot : notnull
{
    public Name Name { get; } = builder.TargetId;
    public Text DisplayName { get; } = builder.TargetDisplayName;
    public Text Tooltip { get; } = builder.TargetTooltip;
    public Text Category { get; } = builder.TargetCategory;
    public TValue? DefaultValue => builder.TargetDefaultValue;
    public Type ClrType => typeof(TValue);

    private readonly Func<TRoot, TValue> _get = builder.TargetGet;
    private readonly Func<TRoot, TValue, TRoot> _with = builder.TargetWith;
    private readonly ImmutableDictionary<string, string> _metadata = builder.TargetMetadata.ToImmutableDictionary();

    public bool HasMetadata(string key)
    {
        return _metadata.ContainsKey(key);
    }

    public string? GetMetadata(string key)
    {
        return _metadata.GetValueOrDefault(key);
    }

    public TValue Get(TRoot root) => _get(root);

    public TRoot With(TRoot root, TValue value) => _with(root, value);

    public abstract TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    );

    public abstract void CollectDiffs(
        TRoot oldRoot,
        TRoot newRoot,
        List<FieldEdit> edits,
        FieldPath basePath,
        JsonSerializerOptions? options = null
    );
}
