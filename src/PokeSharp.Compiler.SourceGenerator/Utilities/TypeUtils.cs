using Microsoft.CodeAnalysis;

namespace PokeSharp.Compiler.SourceGenerator.Utilities;

public static class TypeUtils
{
    public static ITypeSymbol GetUnderlyingType(ITypeSymbol type)
    {
        if (
            type is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol
            && namedTypeSymbol.ConstructUnboundGenericType().ToDisplayString() == "T?"
        )
        {
            return namedTypeSymbol.TypeArguments[0];
        }

        return type;
    }

    public static string GetTargetTypeString(ITypeSymbol type)
    {
        return GetUnderlyingType(type).ToDisplayString(NullableFlowState.NotNull);
    }
}
