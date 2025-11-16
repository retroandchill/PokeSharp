using Microsoft.CodeAnalysis;

namespace PokeSharp.Compiler.SourceGenerator.Model;

internal readonly record struct PbsSubSchema(IPropertySymbol Property, PbsSchema Schema)
{
    public string Name => Property.Name;
    public string SchemaType => Schema.ModelType.ToDisplayString(NullableFlowState.NotNull);
    public bool IsCollection { get; init; }
}
