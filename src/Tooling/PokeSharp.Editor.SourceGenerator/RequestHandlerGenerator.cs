using System.Collections.Immutable;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PokeSharp.Editor.Core;
using PokeSharp.Editor.SourceGenerator.Model;
using PokeSharp.Editor.SourceGenerator.Properties;
using PokeSharp.Editor.SourceGenerator.Utilities;
using Retro.SourceGeneratorUtilities.Utilities.Attributes;
using Retro.SourceGeneratorUtilities.Utilities.Types;

namespace PokeSharp.Editor.SourceGenerator;

[Generator]
public class RequestHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var dataTypes = context
            .SyntaxProvider.ForAttributeWithMetadataName(
                typeof(PokeEditControllerAttribute).FullName!,
                (n, _) => n is ClassDeclarationSyntax,
                (ctx, _) =>
                {
                    var type = (ClassDeclarationSyntax)ctx.TargetNode;
                    return ctx.SemanticModel.GetDeclaredSymbol(type) as INamedTypeSymbol;
                }
            )
            .Where(t => t is not null);

        context.RegisterSourceOutput(dataTypes, Execute!);
    }

    private static void Execute(SourceProductionContext context, INamedTypeSymbol type)
    {
        var controllerInfo = type.GetPokeEditControllerInfo();

        var templateParameters = new
        {
            Namespace = type.ContainingNamespace.ToDisplayString(),
            ClassName = type.Name,
            ControllerName = controllerInfo.Name switch
            {
                not null => controllerInfo.Name,
                null when type.Name.EndsWith("Controller") => type.Name[..^10],
                null => type.Name,
            },
            Methods = type.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m =>
                    !m.IsStatic
                    && m.DeclaredAccessibility == Accessibility.Public
                    && m.HasAttribute<PokeEditRequestAttribute>()
                )
                .Select(GetRequestMethod)
                .ToImmutableArray(),
        };

        var handlebars = Handlebars.Create();
        handlebars.Configuration.TextEncoder = null;

        context.AddSource(
            $"{templateParameters.ClassName}.g.cs",
            handlebars.Compile(SourceTemplates.RequestHandlerTemplate)(templateParameters)
        );
    }

    private static RequestMethodInfo GetRequestMethod(IMethodSymbol method)
    {
        var methodInfo = method.GetPokeEditRequestInfo();

        var name = methodInfo.Name switch
        {
            not null => methodInfo.Name,
            null when method.Name.EndsWith("Async") => method.Name[..^5],
            null => method.Name,
        };

        var isAsync = method.ReturnType.Name is "Task" or "ValueTask";

        ITypeSymbol? returnType;
        if (isAsync)
        {
            if (method.ReturnType is INamedTypeSymbol { IsGenericType: true } namedType)
            {
                returnType = namedType.TypeArguments[0];
            }
            else
            {
                returnType = null;
            }
        }
        else
        {
            returnType = !method.ReturnsVoid ? method.ReturnType : null;
        }

        string? syncName;
        if (isAsync)
        {
            syncName = method.Name.EndsWith("Async") ? method.Name[..^5] : method.Name;
        }
        else
        {
            syncName = method.Name;
        }

        string? responseWriteType;
        if (returnType is not null)
        {
            (responseWriteType, _) = GetReadOrWriteType(returnType);
        }
        else
        {
            responseWriteType = null;
        }

        var validParameters = method.Parameters.Where(p => p.Type.Name != "CancellationToken").ToArray();

        return new RequestMethodInfo
        {
            Name = name,
            SyncName = syncName,
            AsyncName = method.Name,
            IsAsync = isAsync,
            HasCancellationToken =
                method.Parameters.Length > 0 && method.Parameters[^1].Type.Name == "CancellationToken",
            ResponseWriteType = returnType is not null ? responseWriteType : null,
            SerializedResponse = responseWriteType == "Serialized",
            NeedsNullCheck =
                returnType
                    is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.NotAnnotated }
                        or INamedTypeSymbol { IsGenericType: true, MetadataName: "Nullable`1" },
            Parameters = [.. validParameters.Select((x, i) => GetRequestParameter(x, i == validParameters.Length - 1))],
        };
    }

    private static RequestParameterInfo GetRequestParameter(IParameterSymbol parameter, bool isLast)
    {
        var (readType, generic) = GetReadOrWriteType(parameter.Type);

        var needsNullCheck =
            parameter.Type
            is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.NotAnnotated }
                or INamedTypeSymbol { IsGenericType: true, MetadataName: "Nullable`1" };
        return new RequestParameterInfo
        {
            Name = parameter.Name,
            ReadType = readType,
            Generic = generic,
            IsSerialized = readType == "Serialized",
            SyncPassExpression = readType switch
            {
                "String" => $"{parameter.Name}.ToString()",
                "Bytes" => GetBytePassExpression(parameter),
                "Serialized" when needsNullCheck => $"{parameter.Name}.RequireNonNull()",
                _ => parameter.Name,
            },
            AsyncPassExpression = readType switch
            {
                "String" => $"{parameter.Name}.ToString()",
                "Bytes" => GetBytePassExpression(parameter, true),
                "Serialized" when needsNullCheck => $"{parameter.Name}.RequireNonNull()",
                _ => parameter.Name,
            },
            IsLast = isLast,
        };
    }

    private static string GetBytePassExpression(IParameterSymbol parameter, bool isAsync = false)
    {
        if (parameter.Type.IsSameType(typeof(ReadOnlySpan<byte>)))
        {
            return isAsync ? $"{parameter.Name}.Span" : parameter.Name;
        }

        return parameter.Type.IsSameType<ReadOnlyMemory<byte>>() ? parameter.Name : $"{parameter.Name}.ToArray()";
    }

    private static (string TypeName, string Generic) GetReadOrWriteType(ITypeSymbol typeSymbol)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Boolean:
                return ("Boolean", "");
            case SpecialType.System_Byte:
                return ("Byte", "");
            case SpecialType.System_Int32:
                return ("Int32", "");
            case SpecialType.System_Int64:
                return ("Int64", "");
            case SpecialType.System_Single:
                return ("Single", "");
            case SpecialType.System_Double:
                return ("Double", "");
            case SpecialType.System_String:
                return ("String", "");
            default:
            {
                if (typeSymbol.TypeKind == TypeKind.Enum)
                {
                    return ("Enum", $"<{typeSymbol.ToDisplayString()}>");
                }

                if (typeSymbol.IsSameType<Guid>())
                {
                    return ("Guid", "");
                }

                if (typeSymbol.IsSameType(typeof(ReadOnlySpan<char>)) || typeSymbol.IsSameType<ReadOnlyMemory<char>>())
                {
                    return ("String", "");
                }

                var typeName = typeSymbol.ToDisplayString();
                if (typeName == GeneratorConstants.Name)
                {
                    return ("Name", "");
                }

                if (
                    typeSymbol.IsSameType<byte[]>()
                    || typeSymbol.IsSameType<IReadOnlyList<byte>>()
                    || typeSymbol.IsSameType<IReadOnlyCollection<byte>>()
                    || typeSymbol.IsSameType(typeof(ReadOnlySpan<byte>))
                    || typeSymbol.IsSameType<ReadOnlyMemory<byte>>()
                )
                {
                    return ("Bytes", "");
                }

                return ("Serialized", $"<{typeName}>");
            }
        }
    }
}
