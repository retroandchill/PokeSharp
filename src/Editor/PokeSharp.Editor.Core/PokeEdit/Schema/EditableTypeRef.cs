using PokeSharp.Core.Strings;

namespace PokeSharp.Editor.Core.PokeEdit.Schema;

public abstract record EditableTypeRef(EditableTypeKind Kind);

// Primitive: int, bool, string, etc.
public sealed record PrimitiveTypeRef : EditableTypeRef
{
    public PrimitiveKind PrimitiveKind { get; init; }
    public Type ClrType { get; init; }
    public Name Name { get; init; }

    public PrimitiveTypeRef(PrimitiveKind primitiveKind)
        : base(EditableTypeKind.Primitive)
    {
        PrimitiveKind = primitiveKind;
        switch (primitiveKind)
        {
            case PrimitiveKind.Bool:
                (ClrType, Name) = (typeof(bool), "Boolean");
                break;
            case PrimitiveKind.Int8:
                (ClrType, Name) = (typeof(sbyte), "Int8");
                break;
            case PrimitiveKind.Int16:
                (ClrType, Name) = (typeof(short), "Int16");
                break;
            case PrimitiveKind.Int32:
                (ClrType, Name) = (typeof(int), "Int32");
                break;
            case PrimitiveKind.Int64:
                (ClrType, Name) = (typeof(long), "Int64");
                break;
            case PrimitiveKind.UInt8:
                (ClrType, Name) = (typeof(byte), "UInt8");
                break;
            case PrimitiveKind.UInt16:
                (ClrType, Name) = (typeof(ushort), "UInt16");
                break;
            case PrimitiveKind.UInt32:
                (ClrType, Name) = (typeof(uint), "UInt32");
                break;
            case PrimitiveKind.UInt64:
                (ClrType, Name) = (typeof(ulong), "UInt64");
                break;
            case PrimitiveKind.Float:
                (ClrType, Name) = (typeof(float), "Float");
                break;
            case PrimitiveKind.Double:
                (ClrType, Name) = (typeof(double), "Double");
                break;
            case PrimitiveKind.Name:
                (ClrType, Name) = (typeof(Name), "Name");
                break;
            case PrimitiveKind.String:
                (ClrType, Name) = (typeof(string), "String");
                break;
            case PrimitiveKind.Text:
                (ClrType, Name) = (typeof(Text), "Text");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(primitiveKind));
        }
    }

    public static implicit operator PrimitiveTypeRef(PrimitiveKind primitiveKind) => new(primitiveKind);
}

// Enum
public sealed record EnumTypeRef(Type ClrType, Name Name, IReadOnlyDictionary<string, int> Values)
    : EditableTypeRef(EditableTypeKind.Enum);

// Object / record / class, described by a separate EditableType
public sealed record ObjectTypeRef(Name Name) : EditableTypeRef(EditableTypeKind.Object);

// Nullable<T>
public sealed record NullableTypeRef(EditableTypeRef InnerType) : EditableTypeRef(EditableTypeKind.Nullable);

// List<T>
public sealed record ListTypeRef(EditableTypeRef ElementType) : EditableTypeRef(EditableTypeKind.List);

// Dictionary<TKey, TValue>
public sealed record DictionaryTypeRef(EditableTypeRef KeyType, EditableTypeRef ValueType)
    : EditableTypeRef(EditableTypeKind.Dictionary);
