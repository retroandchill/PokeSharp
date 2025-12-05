using System.Collections.Immutable;
using System.Numerics;
using PokeSharp.Core.Strings;
using PokeSharp.Editor.Core.PokeEdit.Schema;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

internal delegate EditablePropertyBuilder<TOwner, TValue> EditablePropertyBuilderFactory<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with
)
    where TOwner : notnull;

public interface IEditablePropertyBuilder<TOwner>
    where TOwner : notnull
{
    Name TargetName { get; }

    IEditableProperty<TOwner> Build(ModelBuildCache cache);
}

public abstract class EditablePropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with,
    TValue? defaultValue
) : IEditablePropertyBuilder<TOwner>
    where TOwner : notnull
{
    protected EditableTypeBuilder<TOwner> TypeBuilder { get; } = typeBuilder;
    public Name TargetName { get; } = name;

    internal Text TargetDisplayName { get; private set; } = name.ToString();
    protected Text TargetTooltip { get; private set; }
    protected Name TargetCategory { get; private set; }
    internal Func<TOwner, TValue> TargetGet { get; } = get;
    internal Func<TOwner, TValue, TOwner> TargetWith { get; } = with;
    internal TValue? TargetDefaultValue { get; private set; } = defaultValue;

    internal abstract FieldDefinition BuildFieldDefinition();

    public EditablePropertyBuilder<TOwner, TValue> DisplayName(Text name)
    {
        TargetDisplayName = name;
        return this;
    }

    public EditablePropertyBuilder<TOwner, TValue> Tooltip(Text tooltip)
    {
        TargetTooltip = tooltip;
        return this;
    }

    public EditablePropertyBuilder<TOwner, TValue> Category(Name category)
    {
        TargetCategory = category;
        return this;
    }

    public EditablePropertyBuilder<TOwner, TValue> DefaultValue(TValue? defaultValue)
    {
        TargetDefaultValue = defaultValue;
        return this;
    }

    public void Ignore()
    {
        TypeBuilder.Properties.Remove(TargetName);
    }

    public abstract IEditableProperty<TOwner> Build(ModelBuildCache cache);
}

public sealed class EditableScalarPropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with,
    TValue? defaultValue
) : EditablePropertyBuilder<TOwner, TValue>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
{
    internal ScalarType ScalarType { get; set; } =
        EditablePropertyBuilder.GetDefaultScalarType<TValue>()
        ?? throw new InvalidOperationException($"Unsupported scalar type {typeof(TValue)}");

    internal ScalarSchemaMetadata TargetMetadata { get; set; } = new();

    internal override FieldDefinition BuildFieldDefinition()
    {
        return EditablePropertyBuilder.CreateForScalarType<TValue>(
            TargetName,
            TargetDisplayName,
            TargetTooltip,
            TargetCategory,
            ScalarType,
            TargetMetadata
        );
    }

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableScalarProperty<TOwner, TValue>(this);
    }
}

public sealed class EditableObjectPropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with,
    TValue? defaultValue
) : EditablePropertyBuilder<TOwner, TValue>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
    where TValue : notnull
{
    internal EditableTypeBuilder<TValue> TargetType => TypeBuilder.ModelBuilder.GetOrCreateBuilder<TValue>();

    internal override FieldDefinition BuildFieldDefinition()
    {
        return new ObjectFieldDefinition
        {
            FieldId = TargetName,
            Label = TargetDisplayName,
            Tooltip = TargetTooltip,
            Category = TargetCategory,
            ObjectTypeId = TargetName,
        };
    }

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableObjectProperty<TOwner, TValue>(this, cache);
    }
}

public abstract class EditableCollectionPropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with,
    TValue? defaultValue
) : EditablePropertyBuilder<TOwner, TValue>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
{
    protected bool TargetFixedSize { get; private set; }
    protected int? TargetMinSize { get; private set; }
    protected int? TargetMaxSize { get; private set; }

    public EditableCollectionPropertyBuilder<TOwner, TValue> FixedSize(bool value = true)
    {
        TargetFixedSize = value;
        return this;
    }

    public EditableCollectionPropertyBuilder<TOwner, TValue> MinSize(int value)
    {
        TargetMinSize = value;
        return this;
    }

    public EditableCollectionPropertyBuilder<TOwner, TValue> MaxSize(int value)
    {
        TargetMaxSize = value;
        return this;
    }
}

public sealed class EditableListPropertyBuilder<TOwner, TItem>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, ImmutableArray<TItem>> get,
    Func<TOwner, ImmutableArray<TItem>, TOwner> with,
    ImmutableArray<TItem> defaultValue
) : EditableCollectionPropertyBuilder<TOwner, ImmutableArray<TItem>>(typeBuilder, name, get, with, defaultValue)
    where TOwner : notnull
    where TItem : notnull
{
    internal EditableTypeBuilder<TItem>? TargetItemType => TypeBuilder.ModelBuilder.GetBuildIfPossible<TItem>();

    internal ScalarType? ScalarType { get; set; } = EditablePropertyBuilder.GetDefaultScalarType<TItem>();

    internal ScalarSchemaMetadata InnerMetadata { get; set; } = new();

    internal override FieldDefinition BuildFieldDefinition()
    {
        return new ListFieldDefinition
        {
            FieldId = TargetName,
            Label = TargetDisplayName,
            Tooltip = TargetTooltip,
            Category = TargetCategory,
            FixedSize = TargetFixedSize,
            MinSize = TargetMinSize,
            MaxSize = TargetMaxSize,
            ItemField = TargetItemType is not null
                ? new ObjectFieldDefinition
                {
                    FieldId = "Inner",
                    Label = Text.None,
                    ObjectTypeId = TargetItemType.TargetId,
                }
                : EditablePropertyBuilder.CreateForScalarType<TItem>(
                    "Inner",
                    Text.None,
                    Text.None,
                    Name.None,
                    ScalarType ?? throw new InvalidOperationException(),
                    InnerMetadata
                ),
        };
    }

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableListProperty<TOwner, TItem>(this, cache);
    }
}

public sealed class EditableDictionaryPropertyBuilder<TOwner, TKey, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    Name name,
    Func<TOwner, ImmutableDictionary<TKey, TValue>> get,
    Func<TOwner, ImmutableDictionary<TKey, TValue>, TOwner> with,
    ImmutableDictionary<TKey, TValue>? defaultValue
)
    : EditableCollectionPropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>>(
        typeBuilder,
        name,
        get,
        with,
        defaultValue
    )
    where TOwner : notnull
    where TKey : notnull
    where TValue : notnull
{
    internal EditableTypeBuilder<TValue>? TargetValueType => TypeBuilder.ModelBuilder.GetBuildIfPossible<TValue>();

    internal ScalarType? KeyScalarType { get; set; } = EditablePropertyBuilder.GetDefaultScalarType<TKey>();

    internal ScalarSchemaMetadata KeyMetadata { get; set; } = new();

    internal ScalarType? ValueScalarType { get; set; } = EditablePropertyBuilder.GetDefaultScalarType<TValue>();

    internal ScalarSchemaMetadata ValueMetadata { get; set; } = new();

    internal override FieldDefinition BuildFieldDefinition()
    {
        return new DictionaryFieldDefinition
        {
            FieldId = TargetName,
            Label = TargetDisplayName,
            Tooltip = TargetTooltip,
            Category = TargetCategory,
            FixedSize = TargetFixedSize,
            MinSize = TargetMinSize,
            MaxSize = TargetMaxSize,
            KeyField = EditablePropertyBuilder.CreateForScalarType<TKey>(
                "Key",
                Text.None,
                Text.None,
                Name.None,
                KeyScalarType ?? throw new InvalidOperationException(),
                KeyMetadata
            ),
            ValueField = TargetValueType is not null
                ? new ObjectFieldDefinition
                {
                    FieldId = "Value",
                    Label = Text.None,
                    ObjectTypeId = TargetValueType.TargetId,
                }
                : EditablePropertyBuilder.CreateForScalarType<TValue>(
                    "Value",
                    Text.None,
                    Text.None,
                    Name.None,
                    ValueScalarType ?? throw new InvalidOperationException(),
                    ValueMetadata
                ),
        };
    }

    public override IEditableProperty<TOwner> Build(ModelBuildCache cache)
    {
        return new EditableDictionaryProperty<TOwner, TKey, TValue>(this, cache);
    }
}

internal static class EditablePropertyBuilder
{
    public static ScalarType? GetDefaultScalarType<TValue>()
    {
        if (typeof(TValue).IsEnum)
        {
            return ScalarType.Choice;
        }

        if (typeof(TValue) == typeof(bool))
        {
            return ScalarType.Bool;
        }

        if (
            typeof(TValue) == typeof(sbyte)
            || typeof(TValue) == typeof(short)
            || typeof(TValue) == typeof(int)
            || typeof(TValue) == typeof(long)
            || typeof(TValue) == typeof(byte)
            || typeof(TValue) == typeof(ushort)
            || typeof(TValue) == typeof(uint)
            || typeof(TValue) == typeof(ulong)
        )
        {
            return ScalarType.Int;
        }

        if (typeof(TValue) == typeof(float) || typeof(TValue) == typeof(double))
        {
            return ScalarType.Float;
        }

        if (typeof(TValue) == typeof(Name) || typeof(TValue) == typeof(string) || typeof(TValue) == typeof(Text))
        {
            return ScalarType.String;
        }

        return null;
    }

    public static FieldDefinition CreateForScalarType<TValue>(
        Name fieldId,
        Text label,
        Text tooltip,
        Name category,
        ScalarType scalarType,
        ScalarSchemaMetadata metadata
    )
    {
        return scalarType switch
        {
            ScalarType.Bool => new BoolFieldDefinition { FieldId = fieldId, Label = label },
            ScalarType.Int => new IntFieldDefinition
            {
                FieldId = fieldId,
                Label = label,
                Tooltip = tooltip,
                Category = category,
                MinValue = (int?)metadata.Min,
                MaxValue = (int?)metadata.Max,
                Step = (int?)metadata.Step,
                DecimalPlaces = metadata.DecimalPlaces,
            },
            ScalarType.Float => new FloatFieldDefinition
            {
                FieldId = fieldId,
                Label = label,
                Tooltip = tooltip,
                Category = category,
                MinValue = metadata.Min,
                MaxValue = metadata.Max,
                Step = metadata.Step,
            },
            ScalarType.String => new TextFieldDefinition
            {
                FieldId = fieldId,
                Label = label,
                Tooltip = tooltip,
                Category = category,
                MaxLength = metadata.MaxLength,
                Regex = metadata.Regex,
                AllowEmpty = metadata.AllowEmpty,
                AllowMultiline = metadata.AllowMultiline,
                IsLocalizable = typeof(TValue) == typeof(Text),
            },
            ScalarType.Choice => new ChoiceFieldDefinition
            {
                FieldId = fieldId,
                Label = label,
                Tooltip = tooltip,
                Category = category,
                AllowNone = !typeof(TValue).IsEnum && metadata.AllowNone,
                Options = metadata.Options!,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(scalarType), scalarType, null),
        };
    }

    extension<TOwner, TValue>(EditablePropertyBuilder<TOwner, TValue> builder)
        where TOwner : notnull
        where TValue : struct, INumber<TValue>
    {
        public EditablePropertyBuilder<TOwner, TValue> Min(TValue value)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, TValue>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { Min = double.CreateChecked(value) };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, TValue> Max(TValue value)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, TValue>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { Max = double.CreateChecked(value) };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, TValue> Step(TValue value)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, TValue>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { Step = double.CreateChecked(value) };
            return builder;
        }
    }

    extension<TOwner, TValue>(EditablePropertyBuilder<TOwner, TValue> builder)
        where TOwner : notnull
        where TValue : struct, INumber<TValue>, IBinaryInteger<TValue>
    {
        public EditablePropertyBuilder<TOwner, TValue> DecimalPlaces(int places)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, TValue>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { DecimalPlaces = places };
            return builder;
        }
    }

    extension<TOwner>(EditablePropertyBuilder<TOwner, Name> builder)
        where TOwner : notnull
    {
        public EditablePropertyBuilder<TOwner, Name> MaxLength(int value)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Name>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { MaxLength = value };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, Name> Regex(string regex)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Name>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { Regex = regex };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, Name> AllowEmpty(bool value = true)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Name>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { AllowEmpty = value };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, Name> AllowMultiline(bool value = true)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Name>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { AllowMultiline = value };
            return builder;
        }
    }

    extension<TOwner>(EditablePropertyBuilder<TOwner, string> builder)
        where TOwner : notnull
    {
        public EditablePropertyBuilder<TOwner, string> MaxLength(int value)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, string>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { MaxLength = value };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, string> Regex(string regex)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, string>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { Regex = regex };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, string> AllowEmpty(bool value = true)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, string>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { AllowEmpty = value };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, string> AllowMultiline(bool value = true)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, string>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { AllowMultiline = value };
            return builder;
        }
    }

    extension<TOwner>(EditablePropertyBuilder<TOwner, Text> builder)
        where TOwner : notnull
    {
        public EditablePropertyBuilder<TOwner, Text> MaxLength(int value)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Text>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { MaxLength = value };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, Text> Regex(string regex)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Text>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { Regex = regex };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, Text> AllowEmpty(bool value = true)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Text>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { AllowEmpty = value };
            return builder;
        }

        public EditablePropertyBuilder<TOwner, Text> AllowMultiline(bool value = true)
        {
            var asScalar = (EditableScalarPropertyBuilder<TOwner, Text>)builder;
            asScalar.TargetMetadata = asScalar.TargetMetadata with { AllowMultiline = value };
            return builder;
        }
    }

    extension<TOwner, TItem>(EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> builder)
        where TOwner : notnull
        where TItem : notnull
    {
        public EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> FixedSize(bool value = true)
        {
            return ((EditableListPropertyBuilder<TOwner, TItem>)builder).FixedSize(value);
        }

        public EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> MinSize(int value)
        {
            return ((EditableListPropertyBuilder<TOwner, TItem>)builder).MinSize(value);
        }

        public EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> MaxSize(int value)
        {
            return ((EditableListPropertyBuilder<TOwner, TItem>)builder).MaxSize(value);
        }
    }

    extension<TOwner, TKey, TValue>(EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> builder)
        where TOwner : notnull
        where TKey : notnull
        where TValue : notnull
    {
        public EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> FixedSize(bool value = true)
        {
            return ((EditableDictionaryPropertyBuilder<TOwner, TKey, TValue>)builder).FixedSize(value);
        }

        public EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> MinSize(int value)
        {
            return ((EditableDictionaryPropertyBuilder<TOwner, TKey, TValue>)builder).MinSize(value);
        }

        public EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> MaxSize(int value)
        {
            return ((EditableDictionaryPropertyBuilder<TOwner, TKey, TValue>)builder).MaxSize(value);
        }
    }
}
