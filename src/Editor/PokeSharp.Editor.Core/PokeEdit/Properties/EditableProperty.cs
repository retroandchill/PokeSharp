using System.Collections.Immutable;
using System.Numerics;
using System.Text.Json;
using PokeSharp.Core.Strings;
using PokeSharp.Core.Utils;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableProperty : IEditableMember
{
    Type ClrType { get; }
}

public interface IEditableProperty<TRoot> : IEditableProperty
    where TRoot : notnull
{
    FieldDefinition GetDefinition(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        JsonSerializerOptions? options = null
    );

    TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
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

    public abstract FieldDefinition GetDefinition(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        JsonSerializerOptions? options = null
    );

    public abstract TRoot ApplyEdit(
        TRoot root,
        ReadOnlySpan<FieldPathSegment> path,
        FieldEdit edit,
        JsonSerializerOptions? options = null
    );
}

internal static class EditableProperty
{
    public static readonly Name Inner = "Inner";

    public static FieldDefinition CreateDefinitionForScalar<TValue>(
        TValue value,
        TValue? defaultValue,
        FieldPathSegment fieldId,
        IEditableMember outer,
        string keyPrefix = "",
        JsonSerializerOptions? options = null
    )
    {
        var isDefault = EqualityComparer<TValue>.Default.Equals(value, defaultValue);
        if (typeof(TValue).IsEnum)
        {
            return new ChoiceFieldDefinition
            {
                FieldId = fieldId,
                Label = outer.DisplayName,
                Tooltip = outer.Tooltip,
                Category = outer.Category,
                CurrentValue = JsonSerializer.SerializeToNode(value, options).RequireNonNull(),
                IsDefaultValue = isDefault,
                AllowNone = false,
                Options = new StaticOptionSourceDefinition()
                {
                    Options =
                    [
                        .. Enum.GetValues(typeof(TValue))
                            .Cast<TValue>()
                            .Select(x => new OptionItemDefinition(
                                JsonSerializer.SerializeToNode(x, options).RequireNonNull(),
                                x.RequireNonNull().ToString().RequireNonNull()
                            )),
                    ],
                },
            };
        }

        return value switch
        {
            bool asBool => new BoolFieldDefinition
            {
                FieldId = fieldId,
                Label = outer.DisplayName,
                Tooltip = outer.Tooltip,
                Category = outer.Category,
                CurrentValue = asBool,
                IsDefaultValue = isDefault,
            },
            sbyte asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            short asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            int asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            long asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            byte asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            ushort asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            uint asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            ulong asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            float asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            double asNum => CreateNumberFieldDefinition(asNum, isDefault, fieldId, outer, keyPrefix),
            Name or string or Text => CreateTextFieldDefinition(
                value.ToString().RequireNonNull(),
                isDefault,
                fieldId,
                outer,
                keyPrefix,
                value is Text
            ),
            _ => throw new InvalidOperationException($"Cannot create definition for scalar type {typeof(TValue)}"),
        };
    }

    private static NumberFieldDefinition<TValue> CreateNumberFieldDefinition<TValue>(
        TValue value,
        bool isDefault,
        FieldPathSegment fieldId,
        IEditableMember outer,
        string keyPrefix
    )
        where TValue : struct, INumber<TValue>
    {
        return new NumberFieldDefinition<TValue>
        {
            FieldId = fieldId,
            Label = outer.DisplayName,
            Tooltip = outer.Tooltip,
            Category = outer.Category,
            CurrentValue = value,
            IsDefaultValue = isDefault,
            MinValue = outer.TryGetNumericMetadata<TValue>($"{keyPrefix}{EditablePropertyBuilder.MinKey}"),
            MaxValue = outer.TryGetNumericMetadata<TValue>($"{keyPrefix}{EditablePropertyBuilder.MaxKey}"),
            Step = outer.TryGetNumericMetadata<TValue>($"{keyPrefix}{EditablePropertyBuilder.StepKey}"),
            DecimalPlaces = outer.TryGetNumericMetadata<int>($"{keyPrefix}{EditablePropertyBuilder.DecimalPlacesKey}"),
        };
    }

    private static TextFieldDefinition CreateTextFieldDefinition(
        string value,
        bool isDefault,
        FieldPathSegment fieldId,
        IEditableMember outer,
        string keyPrefix,
        bool isLocalized
    )
    {
        return new TextFieldDefinition
        {
            FieldId = fieldId,
            Label = outer.DisplayName,
            Tooltip = outer.Tooltip,
            Category = outer.Category,
            CurrentValue = value,
            IsDefaultValue = isDefault,
            MaxLength = outer.TryGetNumericMetadata<int>($"{keyPrefix}{EditablePropertyBuilder.MaxLengthKey}"),
            Regex = outer.GetMetadata($"{keyPrefix}{EditablePropertyBuilder.RegexKey}") ?? "",
            AllowEmpty = outer.TryGetBooleanMetadata($"{keyPrefix}{EditablePropertyBuilder.AllowEmptyKey}", true),
            AllowMultiline = outer.TryGetBooleanMetadata($"{keyPrefix}{EditablePropertyBuilder.AllowMultilineKey}"),
            IsLocalizable = isLocalized,
        };
    }

    extension(IEditableMember outer)
    {
        public bool TryGetBooleanMetadata(string key, bool defaultValue = false)
        {
            return bool.TryParse(outer.GetMetadata(key), out var result) ? result : defaultValue;
        }

        public TValue? TryGetNumericMetadata<TValue>(string key)
            where TValue : struct, INumber<TValue>
        {
            return TValue.TryParse(outer.GetMetadata(key), null, out var result) ? result : null;
        }

        public FieldDefinition CreateValueField<TValue>(IEditableType<TValue>? innerType,
            TValue currentValue,
            TValue? defaultValue,
            FieldPathSegment fieldId,
            ReadOnlySpan<FieldPathSegment> path,
            JsonSerializerOptions? options
        )
            where TValue : notnull
        {
            if (innerType is not null)
            {
                return innerType.GetDefinition(currentValue, fieldId, outer, path[1..], options);
            }

            if (path.Length != 0)
            {
                throw new InvalidOperationException(
                    $"Cannot traverse into scalar property {fieldId}, path still has {path.Length} segment(s)."
                );
            }

            return CreateDefinitionForScalar(currentValue, defaultValue, fieldId, outer, options: options);
        }
    }
}
