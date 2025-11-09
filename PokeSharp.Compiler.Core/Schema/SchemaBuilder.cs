using System.Collections.Immutable;
using System.Reflection;
using PokeSharp.Abstractions;
using PokeSharp.Compiler.Core.Serialization;
using PokeSharp.Compiler.Core.Utils;
using PokeSharp.Core.Data;

namespace PokeSharp.Compiler.Core.Schema;

public class SchemaBuilder
{
    private readonly Dictionary<Type, IReadOnlyDictionary<string, SchemaEntry>> _schemasByType =
        new();
    private readonly Dictionary<Type, SchemaEntry> _subSchemaEntries = new();

    public IReadOnlyDictionary<string, SchemaEntry> BuildSchema(Type targetType)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        if (_schemasByType.TryGetValue(targetType, out var schema))
        {
            return schema;
        }

        if (targetType is { IsClass: false, IsValueType: false })
        {
            throw new ArgumentException(
                $"Type '{targetType.Name}' must be a class or struct.",
                nameof(targetType)
            );
        }

        // Ensure it's not abstract
        if (targetType.IsAbstract)
        {
            throw new ArgumentException(
                $"Type '{targetType.Name}' cannot be abstract.",
                nameof(targetType)
            );
        }

        // Ensure it's not an interface
        if (targetType.IsInterface)
        {
            throw new ArgumentException(
                $"Type '{targetType.Name}' cannot be an interface.",
                nameof(targetType)
            );
        }

        // Ensure it has a parameterless constructor (for classes)
        if (targetType.IsClass && targetType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new ArgumentException(
                $"Type '{targetType.Name}' must have a parameterless constructor.",
                nameof(targetType)
            );
        }

        var schemaInternal = BuildSchemaInternal(targetType);
        _schemasByType.Add(targetType, schemaInternal);
        return schemaInternal;
    }

    private OrderedDictionary<string, SchemaEntry> BuildSchemaInternal(Type targetType)
    {
        var schema = new OrderedDictionary<string, SchemaEntry>();

        foreach (
            var property in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
        )
        {
            var schemaName = GetSchemaName(property);
            var schemaEntry = GetSchemaEntry(property);
            schema.Add(schemaName, schemaEntry);
        }

        return schema;
    }

    private static string GetSchemaName(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<PbsFieldBaseAttribute>();
        return attribute?.Name ?? property.Name;
    }

    private static SchemaEntry GetSchemaEntry(PropertyInfo property)
    {
        var propType = GetUnderlyingType(property.PropertyType);

        var fieldTypes = !TypeUtils.IsSimpleType(propType)
            ? GetComplexFieldType(propType)
            : [GetFieldType(property, propType)];

        return new SchemaEntry(property.Name, fieldTypes)
        {
            FieldStructure = GetFieldStructure(property),
        };
    }

    private static SchemaTypeData GetFieldType(PropertyInfo property, Type propType)
    {
        var typeAttribute = property.GetCustomAttribute<PbsTypeAttribute>();

        if (typeAttribute is null)
            return InferFieldType(propType);

        if (!IsValidFieldType(propType, typeAttribute.FieldType, typeAttribute.EnumType))
        {
            throw new PbsSchemaException(
                $"Property '{property.Name}' has an invalid type. Expected '{typeAttribute.FieldType}' but got '{property.PropertyType}'."
            );
        }

        if (
            typeAttribute.FieldType
            is not (PbsFieldType.Enumerable or PbsFieldType.EnumerableOrInteger)
        )
            return new SchemaTypeData(typeAttribute.FieldType);

        if (typeAttribute.EnumType is null && !propType.IsEnum)
        {
            throw new PbsSchemaException(
                $"Property '{property.Name}' has an enumerable type but no enum type was specified."
            );
        }

        return new SchemaTypeData(
            typeAttribute.FieldType,
            false,
            typeAttribute.EnumType ?? propType,
            typeAttribute.AllowNone
        );
    }

    private static ImmutableArray<SchemaTypeData> GetComplexFieldType(Type propType)
    {
        return
        [
            .. propType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(subProperty =>
                {
                    var subPropType =
                        Nullable.GetUnderlyingType(subProperty.PropertyType)
                        ?? subProperty.PropertyType;
                    return GetFieldType(subProperty, subPropType);
                }),
        ];
    }

    private static bool IsValidFieldType(Type propType, PbsFieldType declaredType, Type? enumType)
    {
        return declaredType switch
        {
            PbsFieldType.Integer => propType == typeof(int)
                || propType == typeof(short)
                || propType == typeof(long)
                || propType == typeof(sbyte)
                || propType == typeof(float)
                || propType == typeof(double)
                || propType == typeof(decimal),
            PbsFieldType.UnsignedInteger
            or PbsFieldType.PositiveInteger
            or PbsFieldType.Hexadecimal => propType == typeof(int)
                || propType == typeof(short)
                || propType == typeof(long)
                || propType == typeof(sbyte)
                || propType == typeof(uint)
                || propType == typeof(ushort)
                || propType == typeof(ulong)
                || propType == typeof(byte)
                || propType == typeof(float)
                || propType == typeof(double)
                || propType == typeof(decimal),
            PbsFieldType.Float => propType == typeof(float)
                || propType == typeof(double)
                || propType == typeof(decimal),
            PbsFieldType.Boolean => propType == typeof(bool),
            PbsFieldType.Name or PbsFieldType.String or PbsFieldType.UnformattedText => propType
                == typeof(string)
                || propType == typeof(Name)
                || propType == typeof(Text),
            PbsFieldType.Symbol => propType == typeof(Name),
            PbsFieldType.Enumerable => IsValidEnumType(propType, enumType),
            PbsFieldType.EnumerableOrInteger => IsValidEnumType(propType, enumType)
                || propType == typeof(int)
                || propType == typeof(short)
                || propType == typeof(long)
                || propType == typeof(sbyte)
                || propType == typeof(float)
                || propType == typeof(double)
                || propType == typeof(decimal),
            _ => throw new ArgumentOutOfRangeException(nameof(declaredType), declaredType, null),
        };
    }

    private static bool IsValidEnumType(Type propType, Type? enumType)
    {
        if (enumType is null)
            return propType.IsEnum;

        var gameDataEntityInterface = enumType
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IGameDataEntity<,>)
            );
        if (gameDataEntityInterface is null)
            return false;

        var keyType = gameDataEntityInterface.GetGenericArguments()[0];
        return propType.IsAssignableFrom(keyType);
    }

    private static SchemaTypeData InferFieldType(Type propType)
    {
        if (propType.IsEnum)
            return new SchemaTypeData(PbsFieldType.Enumerable, false, propType);

        if (
            propType == typeof(int)
            || propType == typeof(short)
            || propType == typeof(long)
            || propType == typeof(sbyte)
        )
            return new SchemaTypeData(PbsFieldType.Integer);

        if (
            propType == typeof(uint)
            || propType == typeof(ushort)
            || propType == typeof(ulong)
            || propType == typeof(byte)
        )
            return new SchemaTypeData(PbsFieldType.UnsignedInteger);

        if (propType == typeof(float) || propType == typeof(double) || propType == typeof(decimal))
            return new SchemaTypeData(PbsFieldType.Float);

        if (propType == typeof(bool))
            return new SchemaTypeData(PbsFieldType.Boolean);

        if (propType == typeof(string) || propType == typeof(Text))
            return new SchemaTypeData(PbsFieldType.String);

        return propType == typeof(Name)
            ? new SchemaTypeData(PbsFieldType.Symbol)
            : new SchemaTypeData(PbsFieldType.UnformattedText);
    }

    private static Type GetUnderlyingType(Type type)
    {
        var nullableUnderlying = Nullable.GetUnderlyingType(type);
        if (nullableUnderlying is not null)
            return nullableUnderlying;

        if (type.IsArray)
            return type.GetElementType()!;

        return type is { IsGenericType: true, IsCollectionType: true }
            ? type.GenericTypeArguments[0]
            : type;
    }

    private static PbsFieldStructure GetFieldStructure(PropertyInfo property)
    {
        if (property.PropertyType.IsCollectionType)
        {
            return property.GetCustomAttribute<PbsKeyRepeatAttribute>() is not null
                ? PbsFieldStructure.Repeating
                : PbsFieldStructure.Array;
        }

        return PbsFieldStructure.Single;
    }
}
