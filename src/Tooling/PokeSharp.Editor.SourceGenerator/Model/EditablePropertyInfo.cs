namespace PokeSharp.Editor.SourceGenerator.Model;

public record EditablePropertyInfo
{
    public required string PropertyClass { get; init; }

    public required string? KeyType { get; init; }
    public required string ValueType { get; init; }
    public bool IsDictionary => KeyType is not null;

    public required string Name { get; init; }

    public string Identifier
    {
        get => field ?? Name;
        init;
    }

    public required string TypeRef { get; init; }
    public required bool IsReadOnly { get; init; }

    public string? DefaultValue { get; init; }
    public bool HasDefaultValue => DefaultValue is not null;
}
