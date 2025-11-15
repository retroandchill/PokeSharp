using System.Collections.Immutable;

namespace PokeSharp.Compiler.Core.Schema;

public static class CollectionTypeHelper
{
    private static readonly ImmutableArray<Type> CollectionInterfaceTypes =
    [
        typeof(IList<>),
        typeof(ICollection<>),
        typeof(IEnumerable<>),
        typeof(IReadOnlyList<>),
        typeof(IReadOnlyCollection<>),
    ];

    // Common concrete collection types
    private static readonly ImmutableArray<Type> ConcreteCollectionTypes =
    [
        typeof(List<>),
        typeof(HashSet<>),
        typeof(LinkedList<>),
        typeof(ImmutableArray<>),
        typeof(ImmutableList<>),
        typeof(ImmutableHashSet<>),
    ];

    extension(Type type)
    {
        public bool IsCollectionType
        {
            get
            {
                if (type.IsArray)
                    return true;

                if (type == typeof(string))
                    return false;

                if (!type.IsGenericType)
                    return false;

                var genericTypeDef = type.GetGenericTypeDefinition();

                if (CollectionInterfaceTypes.Contains(genericTypeDef))
                    return true;

                if (ConcreteCollectionTypes.Contains(genericTypeDef))
                    return true;

                return type.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            }
        }
    }
}
