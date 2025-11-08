using System.Collections.Immutable;

namespace PokeSharp.Compiler.Core.Schema;

public record SchemaEntry(string PropertyName, string TypeString, ImmutableArray<Type?> EnumTypes)
{
    public SchemaEntry(string propertyName, string typeString) : this(propertyName, typeString, [])
    {
        
    }
}