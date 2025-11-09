using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace PokeSharp.Compiler.Core.Serialization.Converters;

public class CollectionConverter : IPbsConverter
{
    public bool CanConvert(string sectionName, PropertyInfo property, object? value)
    {
        if (value is null)
            return false;

        var propertyType = property.PropertyType;
        var valueType = value.GetType();

        // Check if the value implements IEnumerable (but not string)
        if (!typeof(IEnumerable).IsAssignableFrom(valueType) || valueType == typeof(string))
            return false;

        // Check if the property type is one of our supported collection types
        return IsSupportedCollectionType(propertyType);
    }

    public object? Convert(string sectionName, PropertyInfo property, object? value)
    {
        if (value is null)
            return null;

        var targetType = property.PropertyType;
        var enumerable = (IEnumerable)value;

        // Get the element type of the target collection
        var elementType = GetElementType(targetType);
        if (elementType is null)
            throw new InvalidOperationException($"Cannot determine element type for {targetType}");

        // Convert the enumerable to a list of the target element type
        var convertedItems = ConvertElements(enumerable, elementType);

        // Create the appropriate collection type
        return CreateCollection(targetType, elementType, convertedItems);
    }

    private bool IsSupportedCollectionType(Type type)
    {
        if (type.IsArray)
            return true;

        if (type.IsGenericType)
        {
            var genericTypeDef = type.GetGenericTypeDefinition();
            return genericTypeDef == typeof(List<>)
                || genericTypeDef == typeof(HashSet<>)
                || genericTypeDef == typeof(ImmutableArray<>)
                || genericTypeDef == typeof(IList<>)
                || genericTypeDef == typeof(IReadOnlyList<>)
                || genericTypeDef == typeof(ICollection<>)
                || genericTypeDef == typeof(IReadOnlyCollection<>)
                || genericTypeDef == typeof(ISet<>)
                || genericTypeDef == typeof(IReadOnlySet<>)
                || genericTypeDef == typeof(IEnumerable<>);
        }

        return false;
    }

    private Type? GetElementType(Type collectionType)
    {
        // Handle arrays
        if (collectionType.IsArray)
            return collectionType.GetElementType();

        // Handle generic collections
        if (collectionType.IsGenericType)
        {
            var args = collectionType.GetGenericArguments();
            if (args.Length == 1)
                return args[0];
        }

        // Handle IEnumerable implementations
        var enumerableInterface = collectionType
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            );

        return enumerableInterface?.GetGenericArguments().FirstOrDefault();
    }

    private static IList ConvertElements(IEnumerable source, Type targetElementType)
    {
        var listType = typeof(List<>).MakeGenericType(targetElementType);
        var list = (IList)Activator.CreateInstance(listType)!;

        foreach (var item in source)
        {
            if (item is null)
            {
                if (
                    !targetElementType.IsValueType
                    || (
                        targetElementType.IsGenericType
                        && targetElementType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    )
                )
                {
                    list.Add(null);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Cannot assign null to non-nullable type {targetElementType}"
                    );
                }
            }
            else if (targetElementType.IsInstanceOfType(item))
            {
                list.Add(item);
            }
            else
            {
                // Try to convert the item if possible
                var convertedItem = System.Convert.ChangeType(item, targetElementType);
                list.Add(convertedItem);
            }
        }

        return list;
    }

    private object CreateCollection(Type targetType, Type elementType, IList items)
    {
        // Handle arrays
        if (targetType.IsArray)
        {
            var array = Array.CreateInstance(elementType, items.Count);
            items.CopyTo(array, 0);
            return array;
        }

        // Handle generic collections
        if (targetType.IsGenericType)
        {
            var genericTypeDef = targetType.GetGenericTypeDefinition();

            if (
                genericTypeDef == typeof(List<>)
                || genericTypeDef == typeof(IList<>)
                || genericTypeDef == typeof(IReadOnlyList<>)
                || genericTypeDef == typeof(ICollection<>)
                || genericTypeDef == typeof(IReadOnlyCollection<>)
                || genericTypeDef == typeof(IEnumerable<>)
            )
            {
                return items; // items is already a List<T>
            }

            if (
                genericTypeDef == typeof(HashSet<>)
                || genericTypeDef == typeof(ISet<>)
                || genericTypeDef == typeof(IReadOnlySet<>)
            )
            {
                var hashSetType = typeof(HashSet<>).MakeGenericType(elementType);
                var hashSet = Activator.CreateInstance(hashSetType)!;
                var addMethod = hashSetType.GetMethod("Add")!;

                foreach (var item in items)
                {
                    addMethod.Invoke(hashSet, [item]);
                }
                return hashSet;
            }

            if (genericTypeDef == typeof(ImmutableArray<>))
            {
                var createMethod = typeof(ImmutableArray)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(m =>
                        m
                            is {
                                Name: nameof(ImmutableArray.ToImmutableArray),
                                IsGenericMethodDefinition: true
                            }
                    )
                    .FirstOrDefault(m =>
                    {
                        var parameters = m.GetParameters();
                        return parameters is [{ ParameterType.IsGenericType: true }]
                            && parameters[0].ParameterType.GetGenericTypeDefinition()
                                == typeof(IEnumerable<>);
                    });

                if (createMethod != null)
                {
                    var genericMethod = createMethod.MakeGenericMethod(elementType);
                    return genericMethod.Invoke(null, [items])!;
                }
            }
        }

        throw new NotSupportedException($"Collection type {targetType} is not supported");
    }
}
