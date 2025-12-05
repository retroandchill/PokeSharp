using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using PokeSharp.Core.Collections.Immutable;
using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Properties;

public interface IEditableTypeBuilder
{
    Type ClrType { get; }

    IEditableType Build(ModelBuildCache cache);
}

public sealed class EditableTypeBuilder<TOwner>(EditorModelBuilder builder) : IEditableTypeBuilder
    where TOwner : notnull
{
    internal Name TargetId { get; private set; } = typeof(TOwner).Name;
    internal Text TargetDisplayName { get; private set; } = typeof(TOwner).Name;

    internal EditorModelBuilder ModelBuilder { get; } = builder;
    internal readonly OrderedDictionary<Name, IEditablePropertyBuilder<TOwner>> Properties = new();
    public Type ClrType => typeof(TOwner);

    public EditableTypeBuilder<TOwner> DisplayName(Text displayName)
    {
        TargetDisplayName = displayName;
        return this;
    }

    public EditableTypeBuilder<TOwner> Property<TValue>(
        Expression<Func<TOwner, TValue>> propertyGetter,
        Action<EditablePropertyBuilder<TOwner, TValue>>? customize = null
    )
    {
        var property = GetPropertyInfo(propertyGetter);
        return Property(property, customize);
    }

    internal EditableTypeBuilder<TOwner> Property(PropertyInfo property)
    {
        if (Properties.ContainsKey(property.Name))
        {
            return this;
        }

        var createMethod = GetType().GetMethod(nameof(CreateProperty), BindingFlags.NonPublic)!;
        var specialized = createMethod.MakeGenericMethod(property.PropertyType);
        Properties.Add(property.Name, (IEditablePropertyBuilder<TOwner>)specialized.Invoke(this, [property, null])!);
        return this;
    }

    private EditableTypeBuilder<TOwner> Property<TValue>(
        PropertyInfo property,
        Action<EditablePropertyBuilder<TOwner, TValue>>? customize
    )
    {
        if (Properties.TryGetValue(property.Name, out var existingProperty))
        {
            customize?.Invoke((EditablePropertyBuilder<TOwner, TValue>)existingProperty);
        }
        else
        {
            var propertyBuilder = CreateProperty<TValue>(property);
            customize?.Invoke(propertyBuilder);
            Properties.Add(property.Name, propertyBuilder);
        }

        return this;
    }

    private EditablePropertyBuilder<TOwner, TValue> CreateProperty<TValue>(PropertyInfo property)
    {
        if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(ImmutableArray<>))
        {
            var createMethod = GetType().GetMethod(nameof(CreateListProperty), BindingFlags.NonPublic)!;
            var genericMethod = createMethod.MakeGenericMethod(typeof(TValue).GetGenericArguments()[0]);
            return (EditablePropertyBuilder<TOwner, TValue>)genericMethod.Invoke(this, [property])!;
        }

        if (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(ImmutableDictionary<,>))
        {
            var createMethod = GetType().GetMethod(nameof(CreateDictionaryProperty), BindingFlags.NonPublic)!;
            var genericArguments = typeof(TValue).GetGenericArguments();
            var genericMethod = createMethod.MakeGenericMethod(genericArguments[0], genericArguments[1]);
            return (EditablePropertyBuilder<TOwner, TValue>)genericMethod.Invoke(this, [property])!;
        }

        var getter = CompileGetter<TValue>(property);
        var setter = CompileSetter<TValue>(property);

        if (typeof(TValue) == typeof(string))
        {
            return new EditableScalarPropertyBuilder<TOwner, TValue>(
                this,
                property.Name,
                getter,
                setter,
                (TValue)(object)string.Empty
            );
        }

        if (typeof(TValue).IsEnum || EditableTypeBuilder.DefaultableScalars.Contains(typeof(TValue)))
        {
            return new EditableScalarPropertyBuilder<TOwner, TValue>(this, property.Name, getter, setter, default);
        }

        return new EditableObjectPropertyBuilder<TOwner, TValue>(this, property.Name, getter, setter, default);
    }

    private EditablePropertyBuilder<TOwner, ImmutableArray<TItem>> CreateListProperty<TItem>(PropertyInfo property)
    {
        var getter = CompileGetter<ImmutableArray<TItem>>(property);
        var setter = CompileSetter<ImmutableArray<TItem>>(property);

        return new EditableListPropertyBuilder<TOwner, TItem>(this, property.Name, getter, setter, []);
    }

    private EditablePropertyBuilder<TOwner, ImmutableDictionary<TKey, TValue>> CreateDictionaryProperty<TKey, TValue>(
        PropertyInfo property
    )
        where TKey : notnull
    {
        var getter = CompileGetter<ImmutableDictionary<TKey, TValue>>(property);
        var setter = CompileSetter<ImmutableDictionary<TKey, TValue>>(property);

        return new EditableDictionaryPropertyBuilder<TOwner, TKey, TValue>(
            this,
            property.Name,
            getter,
            setter,
            ImmutableDictionary<TKey, TValue>.Empty
        );
    }

    private static Func<TOwner, TValue> CompileGetter<TValue>(PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo.GetMethod);
        return propertyInfo.GetMethod.CreateDelegate<Func<TOwner, TValue>>();
    }

    private static Func<TOwner, TValue, TOwner> CompileSetter<TValue>(PropertyInfo propertyInfo)
    {
        var recordType = propertyInfo.DeclaringType ?? throw new InvalidOperationException();
        var cloneMethod =
            recordType.GetMethod("<Clone>$", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException();
        var cloneDelegate = cloneMethod.CreateDelegate<Func<TOwner, TOwner>>();

        ArgumentNullException.ThrowIfNull(propertyInfo.SetMethod);
        var setDelegate = propertyInfo.SetMethod.CreateDelegate<Action<TOwner, TValue>>();
        return (owner, value) =>
        {
            var clone = cloneDelegate(owner);
            setDelegate(owner, value);
            return clone;
        };
    }

    private static PropertyInfo GetPropertyInfo<TValue>(Expression<Func<TOwner, TValue>> propertyExpression)
    {
        // Handle: x => x.Property
        // Also handle: x => (object)x.Property, x => Convert(x.Property), etc.
        var body = propertyExpression.Body;
        if (body is UnaryExpression { NodeType: ExpressionType.Convert } unary)
        {
            body = unary.Operand;
        }

        if (body is not MemberExpression memberExpression)
        {
            throw new ArgumentException(
                $@"Expression '{propertyExpression}' does not refer to a property.",
                nameof(propertyExpression)
            );
        }

        if (memberExpression.Member is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException(
                $@"Expression '{propertyExpression}' refers to a field, not a property.",
                nameof(propertyExpression)
            );
        }

        // Optional: validate that the property is actually on T or its base type
        var declaringType =
            propertyInfo.DeclaringType
            ?? throw new InvalidOperationException($"Property '{propertyInfo.Name}' has no declaring type.");

        if (!declaringType.IsAssignableFrom(typeof(TOwner)))
        {
            throw new ArgumentException(
                $@"Expression '{propertyExpression}' refers to a property '{propertyInfo.Name}' "
                    + $@"that is not defined on type '{typeof(TOwner)}' or its base types.",
                nameof(propertyExpression)
            );
        }

        return propertyInfo;
    }

    public IEditableType Build(ModelBuildCache cache)
    {
        return new EditableType<TOwner>(this, cache);
    }
}

internal static class EditableTypeBuilder
{
    public static readonly ImmutableArray<Type> DefaultableScalars =
    [
        typeof(bool),
        typeof(sbyte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(byte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(Name),
        typeof(Text),
    ];
}
