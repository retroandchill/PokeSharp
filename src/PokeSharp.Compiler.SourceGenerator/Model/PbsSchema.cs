using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace PokeSharp.Compiler.SourceGenerator.Model;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal record PbsSchema(
    ITypeSymbol ModelType,
    ImmutableArray<PbsSchemaEntry> Properties,
    string FilePath,
    bool IsOptional,
    string? ComparisonFactory
)
{
    public string Namespace => ModelType.ContainingNamespace.ToDisplayString();

    public string ClassName => ModelType.Name;

    public string IsOptionalString => IsOptional ? "true" : "false";

    public string DeclaredAccessibility =>
        ModelType.DeclaredAccessibility switch
        {
            Accessibility.NotApplicable => "",
            Accessibility.Private => "private ",
            Accessibility.ProtectedAndInternal => "private protected ",
            Accessibility.Protected => "protected ",
            Accessibility.Internal => "internal ",
            Accessibility.ProtectedOrInternal => "protected internal ",
            Accessibility.Public => "public ",
            _ => throw new ArgumentOutOfRangeException(),
        };

    public string ObjectType
    {
        get
        {
            if (ModelType.IsRecord)
            {
                return ModelType.IsValueType ? "record struct" : "record";
            }

            return ModelType.IsValueType ? "struct" : "class";
        }
    }

    public bool HasComparisonFactory => ComparisonFactory is not null;
}
