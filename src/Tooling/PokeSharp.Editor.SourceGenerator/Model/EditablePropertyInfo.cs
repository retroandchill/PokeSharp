namespace PokeSharp.Editor.SourceGenerator.Model;

public enum PropertyType
{
    Scalar,
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

    public bool IsList => PropertyType == PropertyType.List;
    public bool IsDictionary => PropertyType == PropertyType.Dictionary;
}
