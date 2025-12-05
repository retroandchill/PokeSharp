using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public sealed class ModelBuildCache
{
    private readonly Dictionary<Type, IEditableType> _types = [];

    [return: NotNullIfNotNull(nameof(builder))]
    public IEditableType? GetOrBuildType(IEditableTypeBuilder? builder)
    {
        if (builder is null)
        {
            return null;
        }

        if (_types.TryGetValue(builder.ClrType, out var type))
        {
            return type;
        }

        var built = builder.Build(this);
        _types.Add(builder.ClrType, built);
        return built;
    }

    [return: NotNullIfNotNull(nameof(builder))]
    public IEditableType<TValue>? GetOrBuildType<TValue>(EditableTypeBuilder<TValue>? builder)
        where TValue : notnull
    {
        return (IEditableType<TValue>?)GetOrBuildType((IEditableTypeBuilder?)builder);
    }

    public ImmutableDictionary<Type, IEditableType> ExportToImmutable()
    {
        return _types.ToImmutableDictionary();
    }
}
