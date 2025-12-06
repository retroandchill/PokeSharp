using System.Collections.Immutable;
using System.Numerics;
using PokeSharp.Core.Strings;

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
    IEditableProperty<TOwner> Build(ModelBuildCache cache);
}

public abstract class EditablePropertyBuilder<TOwner, TValue>(
    EditableTypeBuilder<TOwner> typeBuilder,
    string name,
    Func<TOwner, TValue> get,
    Func<TOwner, TValue, TOwner> with,
    TValue? defaultValue
) : EditableMemberBuilder<EditablePropertyBuilder<TOwner, TValue>>(name), IEditablePropertyBuilder<TOwner>
    where TOwner : notnull
{
    protected EditableTypeBuilder<TOwner> TypeBuilder { get; } = typeBuilder;
    internal Func<TOwner, TValue> TargetGet { get; } = get;
    internal Func<TOwner, TValue, TOwner> TargetWith { get; } = with;
    internal TValue? TargetDefaultValue { get; private set; } = defaultValue;

    public EditablePropertyBuilder<TOwner, TValue> DefaultValue(TValue? defaultValue)
    {
        TargetDefaultValue = defaultValue;
        return this;
    }

    public void Ignore()
    {
        TypeBuilder.Properties.Remove(TargetId);
    }

    public abstract IEditableProperty<TOwner> Build(ModelBuildCache cache);
}

internal static class EditablePropertyBuilder
{
    public const string MinKey = "Min";
    public const string MaxKey = "Max";
    public const string StepKey = "Step";
    public const string DecimalPlacesKey = "Step";

    public const string MaxLengthKey = "MaxLength";
    public const string RegexKey = "Regex";
    public const string AllowEmptyKey = "AllowEmpty";
    public const string AllowMultilineKey = "AllowMultiline";

    public const string FixedSizeKey = "FixedSize";
    public const string MinSizeKey = "MinSize";
    public const string MaxSizeKey = "MaxSize";

    extension<TOwner, TValue>(EditablePropertyBuilder<TOwner, TValue> builder)
        where TOwner : notnull
        where TValue : struct, INumber<TValue>
    {
        public EditablePropertyBuilder<TOwner, TValue> Min(TValue value)
        {
            return builder.Metadata(MinKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, TValue> Max(TValue value)
        {
            return builder.Metadata(MaxKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, TValue> Step(TValue value)
        {
            return builder.Metadata(StepKey, value.ToString());
        }
    }

    extension<TOwner, TValue>(EditablePropertyBuilder<TOwner, TValue> builder)
        where TOwner : notnull
        where TValue : struct, INumber<TValue>, IBinaryInteger<TValue>
    {
        public EditablePropertyBuilder<TOwner, TValue> DecimalPlaces(int places)
        {
            return builder.Metadata(DecimalPlacesKey, places.ToString());
        }
    }

    extension<TOwner>(EditablePropertyBuilder<TOwner, Name> builder)
        where TOwner : notnull
    {
        public EditablePropertyBuilder<TOwner, Name> MaxLength(int value)
        {
            return builder.Metadata(MaxLengthKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, Name> Regex(string regex)
        {
            return builder.Metadata(RegexKey, regex);
        }

        public EditablePropertyBuilder<TOwner, Name> AllowEmpty(bool value = true)
        {
            return builder.Metadata(AllowEmptyKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, Name> AllowMultiline(bool value = true)
        {
            return builder.Metadata(AllowMultilineKey, value.ToString());
        }
    }

    extension<TOwner>(EditablePropertyBuilder<TOwner, string> builder)
        where TOwner : notnull
    {
        public EditablePropertyBuilder<TOwner, string> MaxLength(int value)
        {
            return builder.Metadata(MaxLengthKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, string> Regex(string regex)
        {
            return builder.Metadata(RegexKey, regex);
        }

        public EditablePropertyBuilder<TOwner, string> AllowEmpty(bool value = true)
        {
            return builder.Metadata(AllowEmptyKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, string> AllowMultiline(bool value = true)
        {
            return builder.Metadata(AllowMultilineKey, value.ToString());
        }
    }

    extension<TOwner>(EditablePropertyBuilder<TOwner, Text> builder)
        where TOwner : notnull
    {
        public EditablePropertyBuilder<TOwner, Text> MaxLength(int value)
        {
            return builder.Metadata(MaxLengthKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, Text> Regex(string regex)
        {
            return builder.Metadata(RegexKey, regex);
        }

        public EditablePropertyBuilder<TOwner, Text> AllowEmpty(bool value = true)
        {
            return builder.Metadata(AllowEmptyKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, Text> AllowMultiline(bool value = true)
        {
            return builder.Metadata(AllowMultilineKey, value.ToString());
        }
    }

    extension<TOwner, TItem>(EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> builder)
        where TOwner : notnull
        where TItem : notnull
    {
        public EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> FixedSize(bool value = true)
        {
            return builder.Metadata(FixedSizeKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> MinSize(int value)
        {
            return builder.Metadata(MinSizeKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> MaxSize(int value)
        {
            return builder.Metadata(MaxSizeKey, value.ToString());
        }
    }

    extension<TOwner, TKey, TValue>(EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> builder)
        where TOwner : notnull
        where TKey : notnull
        where TValue : notnull
    {
        public EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> FixedSize(bool value = true)
        {
            return builder.Metadata(FixedSizeKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> MinSize(int value)
        {
            return builder.Metadata(MinSizeKey, value.ToString());
        }

        public EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> MaxSize(int value)
        {
            return builder.Metadata(MaxSizeKey, value.ToString());
        }
    }
}
