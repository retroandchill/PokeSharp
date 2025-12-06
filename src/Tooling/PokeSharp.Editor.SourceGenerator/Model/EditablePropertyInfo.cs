namespace PokeSharp.Editor.SourceGenerator.Model;

public enum PropertyType
{
    Scalar,
    Object,
    List,
    Dictionary,
}

public record EditablePropertyInfo
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required PropertyType PropertyType { get; init; }
    public string? KeyType { get; init; }
    public string ValueType
    {
        get => field ?? Type;
        init;
    }
    public string? ObjectType { get; init; }
    public required string DefaultValue { get; init; }

    public bool IsObject => PropertyType == PropertyType.Object;
    public bool IsList => PropertyType == PropertyType.List;
    public bool IsDictionary => PropertyType == PropertyType.Dictionary;
    public bool IsLast { get; init; }
}
